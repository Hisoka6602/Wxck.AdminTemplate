using System;
using System.IO;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EnumsNamingAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "WXCK005";

        private static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "Enums 文件夹中的类型必须是 enum",
            messageFormat: "文件位于 Enums 文件夹下，但类型 '{0}' 不是 enum 类型",
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

            // 获取文件路径
            var path = namedType.Locations.FirstOrDefault()?.SourceTree?.FilePath;
            if (string.IsNullOrEmpty(path))
                return;

            // 精准匹配目录段名为 "Enums"
            var isInEnumsFolder = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(p => string.Equals(p, "Enums", StringComparison.OrdinalIgnoreCase));

            if (isInEnumsFolder && namedType.TypeKind != TypeKind.Enum) {
                var diagnostic = Diagnostic.Create(Rule, namedType.Locations[0], namedType.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}