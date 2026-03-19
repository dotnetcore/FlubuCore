using FlubuCore.Analyzers.Tests.Scripts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace FlubuCore.Analyzers.Tests
{
    public class ExecuteInAddTaskAnalyzerUnitTests : CodeFixVerifier
    {
        [Fact]
        public void CorrectAddCoreTask_NoDiagnostic()
        {
            VerifyCSharpDiagnostic(ExecuteInAddTaskAnalyzerScripts.CorrectAddCoreTaskScript);
        }

        [Fact]
        public void ExecuteInAddCoreTask_ReportsDiagnostic()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_Execute_001",
                Message = "Do not call 'Execute' inside 'AddCoreTask'. The target executes the task automatically. Remove the Execute call.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 30, 47)
                    }
            };

            VerifyCSharpDiagnostic(ExecuteInAddTaskAnalyzerScripts.ExecuteInAddCoreTaskScript, expected);
        }

        [Fact]
        public void ExecuteInAddTask_ReportsDiagnostic()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_Execute_001",
                Message = "Do not call 'Execute' inside 'AddTask'. The target executes the task automatically. Remove the Execute call.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 31, 43)
                    }
            };

            VerifyCSharpDiagnostic(ExecuteInAddTaskAnalyzerScripts.ExecuteInAddTaskScript, expected);
        }

        [Fact]
        public void ExecuteInAddCoreTaskAsync_ReportsDiagnostic()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_Execute_001",
                Message = "Do not call 'Execute' inside 'AddCoreTaskAsync'. The target executes the task automatically. Remove the Execute call.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 30, 52)
                    }
            };

            VerifyCSharpDiagnostic(ExecuteInAddTaskAnalyzerScripts.ExecuteInAddCoreTaskAsyncScript, expected);
        }

        [Fact]
        public void ExecuteInAddTaskAsync_ReportsDiagnostic()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_Execute_001",
                Message = "Do not call 'Execute' inside 'AddTaskAsync'. The target executes the task automatically. Remove the Execute call.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 30, 48)
                    }
            };

            VerifyCSharpDiagnostic(ExecuteInAddTaskAnalyzerScripts.ExecuteInAddTaskAsyncScript, expected);
        }

        [Fact]
        public void ExecuteAsyncInAddCoreTask_ReportsDiagnostic()
        {
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_Execute_001",
                Message = "Do not call 'ExecuteAsync' inside 'AddCoreTask'. The target executes the task automatically. Remove the ExecuteAsync call.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 31, 47)
                    }
            };

            VerifyCSharpDiagnostic(ExecuteInAddTaskAnalyzerScripts.ExecuteAsyncInAddCoreTaskScript, expected);
        }

        [Fact]
        public void ExecuteInDoMethod_NoDiagnostic()
        {
            VerifyCSharpDiagnostic(ExecuteInAddTaskAnalyzerScripts.ExecuteInDoMethodScript);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ExecuteInAddTaskAnalyzer();
        }
    }
}
