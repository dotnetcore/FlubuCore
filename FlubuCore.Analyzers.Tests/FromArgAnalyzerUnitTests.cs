using System;
using FlubuClore.Analyzer;
using FlubuCore.Analyzers.Tests.Scripts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace FlubuCore.Analyzers.Tests
{
    public class FromArgAnalyzerUnitTests : CodeFixVerifier
    {
        [Fact]
        public void FromArgSupportedPropertyTypeTest()
        {
            VerifyCSharpDiagnostic(FromArgAnalyzerUnitTestsScripts.FromArgSupportedPropertyTypeScript);
        }

        [Fact]
        public void FromArgNotSupportedPropertyTypeTest()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_FromArg_001",
                Message = string.Format("FromArg does not support type '{0}' on property.",  "Object"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 30, 23)
                    }
            };

            VerifyCSharpDiagnostic(FromArgAnalyzerUnitTestsScripts.FromArgNotSupportedPropertyTypeScript, expected);
        }

        [Fact]
        public void FromArgKeyValueShouldNotStartWithDashTest()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_FromArg_002",
                Message = "Key should not start with dash.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 29, 10)
                    }
            };

            VerifyCSharpDiagnostic(FromArgAnalyzerUnitTestsScripts.FromArgKeyValueShouldNotStartWithDashScript, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new FromArgAnalyzer();
        }
    }
}