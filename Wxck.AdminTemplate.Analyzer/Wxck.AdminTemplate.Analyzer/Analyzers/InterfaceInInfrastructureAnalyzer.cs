using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InterfaceInInfrastructureAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "INF001";

        private static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "禁止在 Infrastructure 项目下定义接口",
            messageFormat: "接口 '{0}' 不允许在 Infrastructure 项目下定义。",
            category: "项目结构",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) {
            // 配置分析器
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // 注册对符号的分析
            context.RegisterSymbolAction(AnalyzeSymbolDefinition, SymbolKind.NamedType);
        }

        private void AnalyzeSymbolDefinition(SymbolAnalysisContext context) {
            var symbol = context.Symbol;

            // 检查符号是否为接口
            if (symbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeKind == TypeKind.Interface) {
                // 获取文件路径
                var location = namedTypeSymbol.Locations.FirstOrDefault();
                if (location == null || string.IsNullOrEmpty(location.SourceTree?.FilePath))
                    return;

                var filePath = location.SourceTree.FilePath;

                // 判断文件是否位于 Infrastructure 目录下
                if (filePath.Contains("Infrastructure")) {
                    var diagnostic = Diagnostic.Create(Rule, location, namedTypeSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}