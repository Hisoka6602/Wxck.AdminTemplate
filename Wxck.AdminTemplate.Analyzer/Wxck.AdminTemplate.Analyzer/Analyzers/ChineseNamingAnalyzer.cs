using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ChineseNamingAnalyzer : DiagnosticAnalyzer {

        private static readonly DiagnosticDescriptor NoChineseCharactersRule = new(
            id: "WXCK011",
            title: "禁止使用中文字符命名",
            messageFormat: "类/接口/枚举/属性/字段名 {0} 包含中文字符，禁止使用中文字符。",
            category: "Naming",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(NoChineseCharactersRule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
            context.RegisterSymbolAction(AnalyzeFieldOrProperty, SymbolKind.Field);
            context.RegisterSymbolAction(AnalyzeFieldOrProperty, SymbolKind.Property);
        }

        private void AnalyzeNamedType(SymbolAnalysisContext context) {
            var namedType = (INamedTypeSymbol)context.Symbol;

            // 检查类、接口、枚举的命名是否包含中文字符
            if (ContainsChineseCharacters(namedType.Name)) {
                var diagnostic = Diagnostic.Create(NoChineseCharactersRule, namedType.Locations[0], namedType.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeFieldOrProperty(SymbolAnalysisContext context) {
            var fieldOrProperty = (ISymbol)context.Symbol;

            // 检查字段或属性的命名是否包含中文字符
            if (ContainsChineseCharacters(fieldOrProperty.Name)) {
                var diagnostic = Diagnostic.Create(NoChineseCharactersRule, fieldOrProperty.Locations[0], fieldOrProperty.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        // 检查命名是否包含中文字符
        private bool ContainsChineseCharacters(string name) {
            return name.Any(c => c >= 0x4e00 && c <= 0x9fff); // Unicode 范围为中文字符
        }
    }
}