using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EntityIdRequirementAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "WXCK006";

        private static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "实体类必须包含 Id 属性及其注解",
            messageFormat: "类 '{0}' 位于 Entities 目录下，且未继承 BaseInfoModel，必须包含 public long Id 属性，并添加 [Column(\"Id\"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] 特性",
            category: "数据建模",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "实体类应统一包含主键 Id 及其数据库映射标注");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeEntityClass, SymbolKind.NamedType);
        }

        private void AnalyzeEntityClass(SymbolAnalysisContext context) {
            var classSymbol = (INamedTypeSymbol)context.Symbol;

            // 仅处理类
            if (classSymbol.TypeKind != TypeKind.Class)
                return;

            var filePath = classSymbol.Locations.FirstOrDefault()?.SourceTree?.FilePath;
            if (string.IsNullOrEmpty(filePath))
                return;

            // 判断是否在 Entities 目录下
            var inEntities = filePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                                     .Any(p => string.Equals(p, "Entities", StringComparison.OrdinalIgnoreCase));

            if (!inEntities)
                return;
            // 排除 BaseInfoModel 本身
            if (classSymbol.Name == "BaseInfoModel")
                return;
            // 判断是否继承了 BaseInfoModel 或其基类
            var baseType = classSymbol.BaseType;
            while (baseType != null) {
                if (baseType.Name == "BaseInfoModel")
                    return;
                baseType = baseType.BaseType;
            }

            // 查找 Id 属性
            var idProperty = classSymbol.GetMembers().OfType<IPropertySymbol>()
                .FirstOrDefault(p => p.Name == "Id" &&
                                     p.Type.SpecialType == SpecialType.System_Int64 &&
                                     p.DeclaredAccessibility == Accessibility.Public &&
                                     !p.IsStatic &&
                                     !p.IsReadOnly);

            if (idProperty == null) {
                Report(context, classSymbol);
                return;
            }

            // 检查是否标记了 Column、Key、DatabaseGenerated 属性
            var attributes = idProperty.GetAttributes();
            var hasColumn = attributes.Any(attr => attr.AttributeClass?.Name == "ColumnAttribute" &&
                                                   attr.ConstructorArguments.Any(arg => arg.Value?.ToString() == "Id"));
            var hasKey = attributes.Any(attr => attr.AttributeClass?.Name == "KeyAttribute");
            var hasDatabaseGenerated = attributes.Any(attr =>
                attr.AttributeClass?.Name == "DatabaseGeneratedAttribute" &&
                attr.ConstructorArguments.FirstOrDefault().Value?.ToString() == "Identity");

            if (!hasColumn || !hasKey || !hasDatabaseGenerated) {
                Report(context, classSymbol);
            }
        }

        private void Report(SymbolAnalysisContext context, INamedTypeSymbol classSymbol) {
            var diagnostic = Diagnostic.Create(Rule, classSymbol.Locations[0], classSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}