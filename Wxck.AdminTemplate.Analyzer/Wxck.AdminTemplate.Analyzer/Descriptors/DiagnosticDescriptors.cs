using System;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Wxck.AdminTemplate.Analyzer.Descriptors {

    public static class DiagnosticDescriptors {

        public static readonly DiagnosticDescriptor EntityClassNameMustEndWithInfoModel = new DiagnosticDescriptor(
            id: "WXCK001",
            title: "Entity类需要以'InfoModel'结尾命名",
            messageFormat: "Class '{0}' in 'Entities' folder must end with 'InfoModel'",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}