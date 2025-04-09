using System;
using System.IO;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RepositoriesNamingAnalyzer : DiagnosticAnalyzer {
        public const string DomainRepoRuleId = "WXCK003";
        public const string InfrastructureRepoRuleId = "WXCK004";

        private static readonly DiagnosticDescriptor DomainRepoRule = new(
            id: DomainRepoRuleId,
            title: "Domain 仓储命名规则",
            messageFormat: "Domain 的 Repositories 中的类型 '{0}' 必须是接口且以 'Repository' 结尾",
            category: "Repository",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor InfrastructureRepoRule = new(
            id: InfrastructureRepoRuleId,
            title: "Infrastructure 仓储类命名与特性规则",
            messageFormat: "Infrastructure 的 Repositories 中的类 '{0}' 必须是类、以 'Repository' 结尾，且包含 InjectableRepositoryAttribute特性标记",
            category: "Repository",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DomainRepoRule, InfrastructureRepoRule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private void AnalyzeNamedType(SymbolAnalysisContext context) {
            var namedType = (INamedTypeSymbol)context.Symbol;

            if (namedType.TypeKind != TypeKind.Class && namedType.TypeKind != TypeKind.Interface)
                return;

            var location = namedType.Locations.FirstOrDefault();
            var path = location?.SourceTree?.FilePath;
            if (string.IsNullOrWhiteSpace(path))
                return;

            var projectName = GetProjectNameFromPath(path);

            // 精准匹配 Repositories 目录
            var pathParts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var isInRepositoriesFolder = pathParts.Any(p => string.Equals(p, "Repositories", StringComparison.OrdinalIgnoreCase));

            if (isInRepositoriesFolder && projectName.EndsWith("Domain", StringComparison.OrdinalIgnoreCase)) {
                if (namedType.TypeKind != TypeKind.Interface || !namedType.Name.EndsWith("Repository")) {
                    context.ReportDiagnostic(Diagnostic.Create(DomainRepoRule, location, namedType.Name));
                }
            }

            if (isInRepositoriesFolder && projectName.EndsWith("Infrastructure", StringComparison.OrdinalIgnoreCase)) {
                var hasInjectableAttr = namedType.GetAttributes()
                    .Any(a => a.AttributeClass?.Name == "InjectableRepositoryAttribute" ||
                              a.AttributeClass?.ToDisplayString() == "InjectableRepositoryAttribute");

                // 允许例外类不打标
                if (namedType.TypeKind != TypeKind.Class) {
                    context.ReportDiagnostic(Diagnostic.Create(InfrastructureRepoRule, location, namedType.Name));
                }

                if (!IsRepositoryNameValid(namedType.Name)) // 名字必须以 Repository 或 RepositoryBase 结尾
                {
                    context.ReportDiagnostic(Diagnostic.Create(InfrastructureRepoRule, location, namedType.Name));
                }

                if (!IsExcludedRepository(namedType.Name) && !hasInjectableAttr) {
                    context.ReportDiagnostic(Diagnostic.Create(InfrastructureRepoRule, location, namedType.Name));
                }
            }
        }

        private bool IsRepositoryNameValid(string name) =>
            name.EndsWith("Repository", StringComparison.Ordinal) ||
            name.EndsWith("RepositoryBase", StringComparison.Ordinal);

        private bool IsExcludedRepository(string name) =>
            name is "LocalRepositoryBase" or "MemoryCacheRepositoryBase" or "RepositoryBase";

        // 获取项目名称：假设.cs文件路径在 /xxx/ProjectName/xxx.cs
        private string GetProjectNameFromPath(string filePath) {
            var parts = filePath.Split(Path.DirectorySeparatorChar);
            var repoIndex = parts.ToList().FindIndex(p => string.Equals(p, "Repositories", StringComparison.OrdinalIgnoreCase));

            if (repoIndex > 1) {
                return parts[repoIndex - 1]; // 倒推获取“项目名称”
            }

            // 回退：直接获取倒数第2层目录
            return parts.Length >= 2 ? parts[parts.Length - 2] : "";
        }
    }
}