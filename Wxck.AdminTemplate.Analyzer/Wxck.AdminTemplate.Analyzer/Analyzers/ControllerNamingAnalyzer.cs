using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wxck.AdminTemplate.Analyzer.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ControllerNamingAnalyzer : DiagnosticAnalyzer {

        private static readonly DiagnosticDescriptor ControllerTypeRule = new(
            id: "WXCK007",
            title: "Controller 类型要求",
            messageFormat: "类 {0} 必须是 class 并继承自 ControllerBase",
            category: "Naming",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor ControllerMethodRule = new(
            id: "WXCK008",
            title: "Controller 方法返回类型要求",
            messageFormat: "方法 {0} 的返回类型必须是 JsonResult 或其子类",
            category: "Naming",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(ControllerTypeRule, ControllerMethodRule);

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

            // 判断是否在 Controllers 目录下（确保是精确目录名匹配）
            var pathParts = path.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            var isInControllersFolder = pathParts.Any(p => p.Equals("Controllers", StringComparison.OrdinalIgnoreCase));
            if (!isInControllersFolder)
                return;

            // Exclude WeatherForecastController
            if (namedType.Name == "WeatherForecastController")
                return;

            var baseType = namedType.BaseType;
            var inheritsFromControllerBase = false;

            while (baseType != null) {
                if (baseType.Name == "ControllerBase") {
                    inheritsFromControllerBase = true;
                    break;
                }
                baseType = baseType.BaseType;
            }

            if (!inheritsFromControllerBase) {
                context.ReportDiagnostic(Diagnostic.Create(ControllerTypeRule, namedType.Locations[0], namedType.Name));
                return;
            }

            // 检查方法返回类型
            foreach (var member in namedType.GetMembers().OfType<IMethodSymbol>()) {
                if (member.MethodKind == MethodKind.Constructor)
                    continue;

                var returnType = member.ReturnType;

                if (!IsJsonResultOrDerived(returnType)) {
                    context.ReportDiagnostic(Diagnostic.Create(ControllerMethodRule, member.Locations[0], member.Name));
                }
            }
        }

        private bool IsJsonResultOrDerived(ITypeSymbol returnType) {
            // 检查是否为 JsonResult 或其子类
            if (InheritsFromJsonResult(returnType))
                return true;

            // 检查是否为 Task<JsonResult> 或其子类
            if (returnType is INamedTypeSymbol namedReturnType &&
                namedReturnType.Name == "Task" &&
                namedReturnType.TypeArguments.Length == 1) {
                var taskInnerType = namedReturnType.TypeArguments[0];
                if (InheritsFromJsonResult(taskInnerType))
                    return true;
            }

            return false;
        }

        private bool InheritsFromJsonResult(ITypeSymbol typeSymbol) {
            while (typeSymbol != null) {
                if (typeSymbol.Name == "JsonResult")
                    return true;

                typeSymbol = typeSymbol.BaseType;
            }
            return false;
        }
    }
}