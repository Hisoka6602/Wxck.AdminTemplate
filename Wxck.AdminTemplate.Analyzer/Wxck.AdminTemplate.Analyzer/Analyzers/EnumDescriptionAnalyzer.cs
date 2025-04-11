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
    public class EnumDescriptionAnalyzer : DiagnosticAnalyzer {

        // 定义诊断 ID 和消息格式
        public const string DiagnosticId = "EnumDescription";

        private static readonly LocalizableString Title = "枚举项必须具有 Description 特性";
        private static readonly LocalizableString MessageFormat = "枚举项 '{0}' 必须具有 Description 特性。";
        private static readonly LocalizableString Description = "所有枚举项必须具有 Description 特性。";

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

        // 分析每个语法树中的 Enum 枚举类型
        public override void Initialize(AnalysisContext context) {
            // 禁用代码重写和补全等功能
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            // 针对枚举进行分析
            context.RegisterSyntaxNodeAction(AnalyzeEnum, Microsoft.CodeAnalysis.CSharp.SyntaxKind.EnumDeclaration);
        }

        // 分析枚举的每个成员
        private void AnalyzeEnum(SyntaxNodeAnalysisContext context) {
            if (context.Node is EnumDeclarationSyntax enumDeclaration) {
                // 遍历枚举成员
                foreach (var diagnostic in from enumMember in enumDeclaration.Members
                                           let enumMemberSymbol = context.SemanticModel.GetDeclaredSymbol(enumMember)
                                           where enumMemberSymbol != null && !enumMemberSymbol.GetAttributes()
                             .Any(attribute => attribute.AttributeClass.Name == "DescriptionAttribute")
                                           select Diagnostic.Create(Rule, enumMember.GetLocation(), enumMember.Identifier.Text)) {
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}