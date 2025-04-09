using System;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EventsNamingAnalyzer : DiagnosticAnalyzer {

        private static readonly DiagnosticDescriptor EventNamingRule = new(
            id: "WXCK010",
            title: "类命名必须以 'Event' 结尾",
            messageFormat: "类 {0} 的命名必须以 'Event' 结尾",
            category: "Naming",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(EventNamingRule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private void AnalyzeNamedType(SymbolAnalysisContext context) {
            var namedType = (INamedTypeSymbol)context.Symbol;

            // 只检查类类型
            if (namedType.TypeKind != TypeKind.Class)
                return;

            // 获取文件路径
            var path = namedType.Locations.FirstOrDefault()?.SourceTree?.FilePath;
            if (string.IsNullOrEmpty(path))
                return;

            // 判断是否在 Events 目录下
            var pathParts = path.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            var isInEventsFolder = pathParts.Any(p => p.Equals("Events", StringComparison.OrdinalIgnoreCase));
            if (!isInEventsFolder)
                return;

            // 检查类名是否以 "Event" 结尾
            if (!namedType.Name.EndsWith("Event", StringComparison.OrdinalIgnoreCase)) {
                context.ReportDiagnostic(Diagnostic.Create(EventNamingRule, namedType.Locations[0], namedType.Name));
            }
        }
    }
}