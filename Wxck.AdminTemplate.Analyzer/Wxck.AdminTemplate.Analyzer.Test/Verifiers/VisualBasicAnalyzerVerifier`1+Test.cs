﻿using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.VisualBasic.Testing;

namespace Wxck.AdminTemplate.Analyzer.Test {
    public static partial class VisualBasicAnalyzerVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new() {
        public class Test : VisualBasicAnalyzerTest<TAnalyzer, MSTestVerifier> {
            public Test() {
            }
        }
    }
}
