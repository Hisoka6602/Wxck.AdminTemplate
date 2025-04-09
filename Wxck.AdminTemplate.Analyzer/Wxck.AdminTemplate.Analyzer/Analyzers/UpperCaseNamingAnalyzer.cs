using System;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UpperCaseNamingAnalyzer : DiagnosticAnalyzer {

        private static readonly DiagnosticDescriptor NameStartsWithUpperCaseRule = new(
            id: "WXCK010",
            title: "命名规则要求",
            messageFormat: "类/接口/字段名 {0} 必须以大写字母开头。",
            category: "Naming",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(NameStartsWithUpperCaseRule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
            context.RegisterSymbolAction(AnalyzeField, SymbolKind.Field);
        }

        private void AnalyzeNamedType(SymbolAnalysisContext context) {
            var namedType = (INamedTypeSymbol)context.Symbol;

            // 检查类和接口的命名是否以大写字母开头
            if (namedType.TypeKind == TypeKind.Class || namedType.TypeKind == TypeKind.Interface) {
                if (!StartsWithUpperCase(namedType.Name)) {
                    var diagnostic = Diagnostic.Create(NameStartsWithUpperCaseRule, namedType.Locations[0], namedType.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private void AnalyzeField(SymbolAnalysisContext context) {
            var field = (IFieldSymbol)context.Symbol;

            // 仅检查 public 字段的命名
            if (field.DeclaredAccessibility == Accessibility.Public) {
                // 检查字段的命名是否以大写字母开头
                if (!StartsWithUpperCase(field.Name)) {
                    var diagnostic = Diagnostic.Create(NameStartsWithUpperCaseRule, field.Locations[0], field.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        // 判断名字是否以大写字母开头
        private bool StartsWithUpperCase(string name) {
            return !string.IsNullOrEmpty(name) && Char.IsUpper(name[0]);
        }
    }
}