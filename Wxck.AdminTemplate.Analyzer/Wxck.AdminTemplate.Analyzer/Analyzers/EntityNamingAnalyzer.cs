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
            messageFormat: "位于 Attributes 文件夹下的类 '{0}' 必须以 'Attribute' 结尾且继承自 System.Attribute 或其派生类",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule, AttributeRule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private void AnalyzeNamedType(SymbolAnalysisContext context) {
            var namedType = (INamedTypeSymbol)context.Symbol;

            if (namedType.TypeKind != TypeKind.Class)
                return;

            var path = namedType.Locations.FirstOrDefault()?.SourceTree?.FilePath;
            if (string.IsNullOrEmpty(path))
                return;

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
                var isAttributeOrDerived = InheritsFromAttribute(namedType);
                var nameCorrect = namedType.Name.EndsWith("Attribute");

                if (!isAttributeOrDerived || !nameCorrect) {
                    var diagnostic = Diagnostic.Create(AttributeRule, namedType.Locations[0], namedType.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        // 检查是否继承自 System.Attribute 或其派生类（递归检查所有基类）
        private static bool InheritsFromAttribute(INamedTypeSymbol symbol) {
            var baseType = symbol.BaseType;
            while (baseType != null) {
                if (IsAttributeType(baseType))
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }

        private static bool IsAttributeType(INamedTypeSymbol symbol) {
            var fullName = symbol.ToDisplayString();
            return fullName == "System.Attribute" ||
                   fullName == "System.ComponentModel.DataAnnotations.ValidationAttribute";
        }
    }
}