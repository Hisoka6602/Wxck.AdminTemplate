using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DtoNamingAnalyzer : DiagnosticAnalyzer {

        private static readonly DiagnosticDescriptor RequestDtoRule = new(
            id: "DTO002",
            title: "RequestModels 类命名不规范",
            messageFormat: "位于 'RequestModels' 目录下的类 '{0}' 必须以 'RequestDto' 结尾",
            category: "命名规范",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "RequestModels 目录下的类必须以 RequestDto 结尾。");

        private static readonly DiagnosticDescriptor ResponseDtoRule = new(
            id: "DTO003",
            title: "ResponseModels 类命名不规范",
            messageFormat: "位于 'ResponseModels' 目录下的类 '{0}' 必须以 'ResponseDto' 结尾",
            category: "命名规范",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "ResponseModels 目录下的类必须以 ResponseDto 结尾。");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(RequestDtoRule, ResponseDtoRule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private void AnalyzeNamedType(SymbolAnalysisContext context) {
            var namedType = (INamedTypeSymbol)context.Symbol;

            if (namedType.TypeKind != TypeKind.Class)
                return;

            var filePath = namedType.Locations.FirstOrDefault()?.SourceTree?.FilePath;
            if (string.IsNullOrEmpty(filePath))
                return;

            var normalizedPath = filePath.Replace('\\', '/');

            if (normalizedPath.Contains("/DTOs/RequestModels")) {
                if (!namedType.Name.EndsWith("RequestDto")) {
                    var diagnostic = Diagnostic.Create(
                        RequestDtoRule,
                        namedType.Locations[0],
                        namedType.Name);

                    context.ReportDiagnostic(diagnostic);
                }
            }

            if (normalizedPath.Contains("/DTOs/ResponseModels")) {
                if (!namedType.Name.EndsWith("ResponseDto")) {
                    var diagnostic = Diagnostic.Create(
                        ResponseDtoRule,
                        namedType.Locations[0],
                        namedType.Name);

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}