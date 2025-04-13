using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EntityPropertyCommentAnalyzer : DiagnosticAnalyzer {

        public static readonly DiagnosticDescriptor MissingPropertyCommentRule = new(
            id: "WXCK004",
            title: "实体字段缺少注释",
            messageFormat: "属性 '{0}' 缺少注释，请添加 /// <summary> 注释",
            category: "注释规范",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Entities 目录下的所有类属性必须包含 XML 注释，以提升代码可读性。");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(MissingPropertyCommentRule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        }

        private void AnalyzeProperty(SyntaxNodeAnalysisContext context) {
            var propertyDecl = (PropertyDeclarationSyntax)context.Node;

            var classDecl = propertyDecl.Parent as ClassDeclarationSyntax;
            if (classDecl == null)
                return;

            var tree = context.Node.SyntaxTree;
            var filePath = tree.FilePath;
            if (string.IsNullOrEmpty(filePath))
                return;

            var isInEntities = filePath.Split(Path.DirectorySeparatorChar)
                .Any(p => string.Equals(p, "Entities", StringComparison.OrdinalIgnoreCase));
            if (!isInEntities)
                return;

            // 使用字符串判断是否包含 XML 注释标记
            var leadingTriviaText = propertyDecl.GetLeadingTrivia().ToFullString();
            var hasXmlComment = leadingTriviaText.Contains("/// <summary>");

            if (!hasXmlComment) {
                var propertyName = propertyDecl.Identifier.Text;
                var diagnostic = Diagnostic.Create(MissingPropertyCommentRule, propertyDecl.GetLocation(), propertyName);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}