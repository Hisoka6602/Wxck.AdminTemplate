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
    public class UtilsStaticClassAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "WXCK004";

        private static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "Utils 目录下必须定义静态类",
            messageFormat: "类 '{0}' 位于 Utils 目录下，必须定义为 static 类",
            category: "命名规范",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Utils 目录下应该只包含工具类，因此类必须使用 static 修饰。");

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
            bool isInUtils = directories.Any(d => string.Equals(d, "Utils", StringComparison.OrdinalIgnoreCase));

            if (isInUtils && !namedType.IsStatic) {
                var diagnostic = Diagnostic.Create(Rule, namedType.Locations[0], namedType.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}