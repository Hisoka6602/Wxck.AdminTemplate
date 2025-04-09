using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FilterNamingAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "WXCK006";

        private static readonly DiagnosticDescriptor Rule = new(
       id: DiagnosticId,
       title: "Filter 命名和继承规范",
       messageFormat: "Filter 文件夹下的类 '{0}' 必须以 'Attribute' 结尾，且继承自 ActionFilterAttribute",
       category: "Naming",
       defaultSeverity: DiagnosticSeverity.Error,
       isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private void AnalyzeNamedType(SymbolAnalysisContext context) {
            var namedType = (INamedTypeSymbol)context.Symbol;

            if (namedType.TypeKind != TypeKind.Class)
                return;

            var location = namedType.Locations.FirstOrDefault();
            var path = location?.SourceTree?.FilePath ?? string.Empty;

            // 精确匹配路径段（避免 Warmup、PlatformFilter 等非目标文件夹被误匹配）
            var isInFilterFolder = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(p => string.Equals(p, "Filter", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(p, "Filters", StringComparison.OrdinalIgnoreCase));

            if (!isInFilterFolder)
                return;

            var nameCorrect = namedType.Name.EndsWith("Attribute");

            var inheritsActionFilter = false;
            var baseType = namedType.BaseType;
            while (baseType != null) {
                if (baseType.ToDisplayString() == "Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute") {
                    inheritsActionFilter = true;
                    break;
                }
                baseType = baseType.BaseType;
            }

            if (!nameCorrect || !inheritsActionFilter) {
                var diagnostic = Diagnostic.Create(Rule, location, namedType.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}