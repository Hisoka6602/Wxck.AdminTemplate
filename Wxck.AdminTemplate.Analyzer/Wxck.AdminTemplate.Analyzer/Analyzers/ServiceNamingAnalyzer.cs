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
    public class ServiceNamingAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "WXCK003";

        private static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "Services 目录下的类必须以 'Service' 结尾",
            messageFormat: "类 '{0}' 位于 Services 目录下，命名必须以 'Service' 结尾",
            category: "命名规范",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "为了保持命名一致性，Services 目录下的类必须以 'Service' 为后缀。");

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
            bool isInServices = directories.Any(d => string.Equals(d, "Services", StringComparison.OrdinalIgnoreCase));

            if (isInServices && !namedType.Name.EndsWith("Service")) {
                var diagnostic = Diagnostic.Create(Rule, namedType.Locations[0], namedType.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}