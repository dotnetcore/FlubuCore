using FlubuCore.Analyzers.Tests.Scripts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace FlubuCore.Analyzers.Tests
{
    public class MissingExecuteAnalyzerUnitTests : CodeFixVerifier
    {
        [Fact]
        public void CorrectExecuteInChain_NoDiagnostic()
        {
            VerifyCSharpDiagnostic(MissingExecuteAnalyzerScripts.CorrectExecuteInChainScript);
        }

        [Fact]
        public void CorrectExecuteOnVariable_NoDiagnostic()
        {
            VerifyCSharpDiagnostic(MissingExecuteAnalyzerScripts.CorrectExecuteOnVariableScript);
        }

        [Fact]
        public void CorrectExecuteAfterFluentOnVariable_NoDiagnostic()
        {
            VerifyCSharpDiagnostic(MissingExecuteAnalyzerScripts.CorrectExecuteAfterFluentOnVariableScript);
        }

        [Fact]
        public void CorrectReassignedVariable_NoDiagnostic()
        {
            VerifyCSharpDiagnostic(MissingExecuteAnalyzerScripts.CorrectReassignedVariableScript);
        }

        [Fact]
        public void MissingExecuteInChain_ReportsDiagnostic()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_Execute_002",
                Message = "Task created via 'RunProgramTask' is never executed. Call Execute(context) or ExecuteAsync(context) on the task.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 28, 13)
                    }
            };

            VerifyCSharpDiagnostic(MissingExecuteAnalyzerScripts.MissingExecuteInChainScript, expected);
        }

        [Fact]
        public void MissingExecuteOnVariable_ReportsDiagnostic()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_Execute_002",
                Message = "Task created via 'RunProgramTask' is never executed. Call Execute(context) or ExecuteAsync(context) on the task.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 28, 24)
                    }
            };

            VerifyCSharpDiagnostic(MissingExecuteAnalyzerScripts.MissingExecuteOnVariableScript, expected);
        }

        [Fact]
        public void MissingExecuteDiscarded_ReportsDiagnostic()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_Execute_002",
                Message = "Task created via 'Build' is never executed. Call Execute(context) or ExecuteAsync(context) on the task.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 27, 13)
                    }
            };

            VerifyCSharpDiagnostic(MissingExecuteAnalyzerScripts.MissingExecuteDiscardedScript, expected);
        }

        [Fact]
        public void MissingExecuteCoreTasks_ReportsDiagnostic()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_Execute_002",
                Message = "Task created via 'Build' is never executed. Call Execute(context) or ExecuteAsync(context) on the task.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 28, 13)
                    }
            };

            VerifyCSharpDiagnostic(MissingExecuteAnalyzerScripts.MissingExecuteCoreTasksScript, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MissingExecuteAnalyzer();
        }
    }
}
