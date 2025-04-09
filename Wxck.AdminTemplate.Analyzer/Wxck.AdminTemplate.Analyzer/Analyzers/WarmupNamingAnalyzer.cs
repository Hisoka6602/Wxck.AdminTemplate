using System;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WarmupNamingAnalyzer : DiagnosticAnalyzer {

        private static readonly DiagnosticDescriptor StartupFilterRule = new(
            id: "WXCK009",
            title: "Warmup 目录中的类继承 IStartupFilter",
            messageFormat: "类 {0} 必须继承自 IStartupFilter",
            category: "Naming",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(StartupFilterRule);

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

            // 判断是否在 Warmup 目录下
            var pathParts = path.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            var isInWarmupFolder = pathParts.Any(p => p.Equals("Warmup", StringComparison.OrdinalIgnoreCase));
            if (!isInWarmupFolder)
                return;

            // 检查是否实现了 IStartupFilter 接口
            if (!namedType.AllInterfaces.Any(i => i.ToDisplayString() == "Microsoft.AspNetCore.Hosting.IStartupFilter")) {
                context.ReportDiagnostic(Diagnostic.Create(StartupFilterRule, namedType.Locations[0], namedType.Name));
            }
        }
    }
}