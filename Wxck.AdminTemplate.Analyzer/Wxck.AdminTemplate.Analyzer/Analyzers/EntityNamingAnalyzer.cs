using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#pragma warning disable RS1005

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EntityNamingAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "WXCK001";
        public const string AttributeDiagnosticId = "WXCK002";

        private static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "Entity 类命名规则",
            messageFormat: "类 '{0}' 必须以 'InfoModel' 结尾",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor AttributeRule = new(
            id: AttributeDiagnosticId,
            title: "Attribute 命名规则",
            messageFormat: "位于 Attributes 文件夹下的类 '{0}' 必须以 'Attribute' 结尾且继承自 System.Attribute",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule, AttributeRule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private void AnalyzeNamedType(SymbolAnalysisContext context) {
            var namedType = (INamedTypeSymbol)context.Symbol;

            // 只检查 class 类型
            if (namedType.TypeKind != TypeKind.Class)
                return;

            // 获取文件路径
            var path = namedType.Locations.FirstOrDefault()?.SourceTree?.FilePath;
            if (string.IsNullOrEmpty(path))
                return;

            // 分割路径目录，用于精确判断目录名
            var pathParts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            bool isInEntities = pathParts.Any(p => string.Equals(p, "Entities", StringComparison.OrdinalIgnoreCase));
            bool isInAttributes = pathParts.Any(p => string.Equals(p, "Attributes", StringComparison.OrdinalIgnoreCase));

            if (isInEntities) {
                if (!namedType.Name.EndsWith("InfoModel")) {
                    var diagnostic = Diagnostic.Create(Rule, namedType.Locations[0], namedType.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }

            if (isInAttributes) {
                var isAttribute = namedType.BaseType?.ToDisplayString() == "System.Attribute";
                var nameCorrect = namedType.Name.EndsWith("Attribute");

                if (!isAttribute || !nameCorrect) {
                    var diagnostic = Diagnostic.Create(AttributeRule, namedType.Locations[0], namedType.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}