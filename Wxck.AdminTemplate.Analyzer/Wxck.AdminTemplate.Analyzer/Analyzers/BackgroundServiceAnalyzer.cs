using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BackgroundServiceAnalyzer : DiagnosticAnalyzer {

        private static readonly DiagnosticDescriptor Rule = new(
            id: "BG0001",
            title: "BackgroundService 必须位于 BackgroundServices 文件夹下并带有 HostedServiceAttribute 特性",
            messageFormat: "类 '{0}' 应该位于 'BackgroundServices' 目录下并且带有 HostedServiceAttribute 特性",
            category: "Usage",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private void AnalyzeNamedType(SymbolAnalysisContext context) {
            // 获取当前符号是类类型
            var symbol = (INamedTypeSymbol)context.Symbol;

            // 检查符号是否是 BackgroundService 的子类
            if (symbol.InheritsFrom("BackgroundService")) {
                // 获取文件路径
                var filePath = context.Compilation.SyntaxTrees
                    .FirstOrDefault(x => x.FilePath == context.Symbol.Locations.FirstOrDefault()?.SourceTree?.FilePath)?.FilePath;

                // 确保类文件位于 BackgroundServices 文件夹下
                if (filePath != null && !filePath.Contains("BackgroundServices")) {
                    var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }

                // 确保类包含 HostedServiceAttribute 特性
                var hasAttribute = symbol.GetAttributes()
                    .Any(attr => attr.AttributeClass?.Name == "HostedServiceAttribute");

                if (!hasAttribute) {
                    var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    // 扩展方法，检查类是否继承自指定的基类
    public static class SymbolExtensions {

        public static bool InheritsFrom(this INamedTypeSymbol symbol, string baseClassName) {
            var baseType = symbol.BaseType;
            while (baseType != null) {
                if (baseType.Name == baseClassName)
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }
    }
}