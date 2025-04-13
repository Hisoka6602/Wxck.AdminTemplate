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
    public class ExtensionsNamingAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "WXCK005";

        private static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "Extensions 目录命名规则",
            messageFormat: "类 '{0}' 位于 Extensions 目录下，必须以 'Extensions' 结尾",
            category: "命名规范",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "所有位于 Extensions 目录下的类都应以 Extensions 为后缀命名，以体现其扩展方法用途。");

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
            if (location == null || !location.IsInSource)
                return;

            var filePath = location.SourceTree?.FilePath;
            if (string.IsNullOrEmpty(filePath))
                return;

            var directories = filePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            bool isInExtensions = directories.Any(d => string.Equals(d, "Extensions", StringComparison.OrdinalIgnoreCase));

            if (isInExtensions && !namedType.Name.EndsWith("Extensions")) {
                var diagnostic = Diagnostic.Create(Rule, namedType.Locations[0], namedType.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}