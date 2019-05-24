using System;
using FlubuCore.Analyzers.Tests.Scripts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace FlubuCore.Analyzers.Tests
{
    public class TargetParameterAnalyzerUnitTests : CodeFixVerifier
    {
        [Fact]
        public void CorrectTargetDefinititionTest()
        {
            VerifyCSharpDiagnostic(TargetParameterAnalyzerUnitTestsScripts.CorrectTargetDefinititionScript);
        }

        [Fact]
        public void WrongFirstParameterTest()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_TargetParameter_001",
                Message = string.Format("First parameter in method '{0}' must be of type ITarget.", "SuccesfullTarget"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 24, 21)
                    }
            };

            VerifyCSharpDiagnostic(TargetParameterAnalyzerUnitTestsScripts.WrongFirstParameterScript, expected);
        }

        [Fact]
        public void NoFirstParameterTest()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_TargetParameter_001",
                Message = string.Format("First parameter in method '{0}' must be of type ITarget.", "SuccesfullTarget"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 24, 21)
                    }
            };

            VerifyCSharpDiagnostic(TargetParameterAnalyzerUnitTestsScripts.NoFirstParameterScript, expected);
        }

        [Fact]
        public void WrongParameterCountTest()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_TargetParameter_002",
                Message = string.Format("Parameters count in attribute and  method '{0}' must be the same.", "SuccesfullTarget"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 35, 14)
                    }
            };

            VerifyCSharpDiagnostic(TargetParameterAnalyzerUnitTestsScripts.WrongParameterCountScript, expected);
        }

        [Fact]
        public void AttributeDoesntHaveParametersMethodDoesTest()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_TargetParameter_002",
                Message = string.Format("Parameters count in attribute and  method '{0}' must be the same.", "SuccesfullTarget"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 35, 10)
                    }
            };

            VerifyCSharpDiagnostic(TargetParameterAnalyzerUnitTestsScripts.AttributeDoesntHaveParametersMethodDoesScript, expected);
        }

        [Fact]
        public void AttributeAndMethodParameterCountAreTheSameTest()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_TargetParameterAnalyzer",
                Message = string.Format("Parameters count in attribute and  method '{0}' must be the same.", "SuccesfullTarget"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 36, 10)
                    }
            };

            VerifyCSharpDiagnostic(TargetParameterAnalyzerUnitTestsScripts.AttributeAndMethodParameterCountAreTheSameScript);
        }

        [Fact]
        public void WrongParameterTypeTest()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_TargetParameter_003",
                Message = string.Format("Parameter must be of same type as '{0}' method parameter '{1}'.", "SuccesfullTarget",  "path"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 35, 35)
                    }
            };

            VerifyCSharpDiagnostic(TargetParameterAnalyzerUnitTestsScripts.WrongParameterType, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new TargetParameterAnalyzer();
        }
    }
}
