using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InterfaceMethodDocumentationAnalyzer : DiagnosticAnalyzer {

        // 定义诊断 ID 和消息格式
        public const string DiagnosticId = "InterfaceMethodDocumentation";

        private static readonly LocalizableString Title = "接口方法必须包含注释";
        private static readonly LocalizableString MessageFormat = "接口方法 '{0}' 必须包含注释文档";
        private static readonly LocalizableString Description = "所有接口定义的函数必须包含注释文档。";

        // 创建诊断规则
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            "命名规范",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        // 分析每个语法树中的接口定义
        public override void Initialize(AnalysisContext context) {
            // 禁用代码重写和补全等功能
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            // 针对接口定义进行分析
            context.RegisterSyntaxNodeAction(AnalyzeInterface, Microsoft.CodeAnalysis.CSharp.SyntaxKind.InterfaceDeclaration);
        }

        // 分析接口中每个方法的注释
        private void AnalyzeInterface(SyntaxNodeAnalysisContext context) {
            if (context.Node is InterfaceDeclarationSyntax interfaceDeclaration) {
                // 遍历接口中的所有方法
                foreach (var member in interfaceDeclaration.Members) {
                    if (member is MethodDeclarationSyntax methodDeclaration) {
                        // 检查方法是否有注释
                        var triviaList = methodDeclaration.GetLeadingTrivia();

                        // 如果方法没有注释，报告诊断
                        if (!triviaList.Any(trivia => trivia.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.SingleLineCommentTrivia) ||
                                                      trivia.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.MultiLineCommentTrivia))) {
                            // 创建诊断对象
                            var diagnostic = Diagnostic.Create(Rule, methodDeclaration.GetLocation(), methodDeclaration.Identifier.Text);
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }
        }
    }
}