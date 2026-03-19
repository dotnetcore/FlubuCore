using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FlubuCore.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExecuteInAddTaskAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FlubuCore_Execute_001";

        private const string Category = "TaskExecution";

        private static readonly LocalizableString Title = "Execute called inside AddTask/AddCoreTask";

        private static readonly string MessageFormat =
            "Do not call '{0}' inside '{1}'. The target executes the task automatically. Remove the {0} call.";

        private static readonly string Description =
            "Tasks added via AddTask/AddCoreTask are executed automatically by the target. Calling Execute manually causes double execution.";

        private static readonly DiagnosticDescriptor _executeInAddTask = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
            isEnabledByDefault: true, description: Description);

        private static readonly HashSet<string> ExecuteMethodNames = new HashSet<string>
        {
            "Execute",
            "ExecuteAsync",
            "ExecuteVoid",
            "ExecuteVoidAsync"
        };

        private static readonly HashSet<string> AddTaskMethodNames = new HashSet<string>
        {
            "AddTask",
            "AddCoreTask",
            "AddTaskAsync",
            "AddCoreTaskAsync"
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(_executeInAddTask);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            string executeMethodName = GetMethodName(invocation);
            if (executeMethodName == null || !ExecuteMethodNames.Contains(executeMethodName))
            {
                return;
            }

            string addTaskMethodName = FindEnclosingAddTaskLambda(invocation);
            if (addTaskMethodName == null)
            {
                return;
            }

            var diagnostic = Diagnostic.Create(_executeInAddTask, invocation.GetLocation(), executeMethodName, addTaskMethodName);
            context.ReportDiagnostic(diagnostic);
        }

        private static string GetMethodName(InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                return memberAccess.Name.Identifier.Text;
            }

            if (invocation.Expression is IdentifierNameSyntax identifier)
            {
                return identifier.Identifier.Text;
            }

            return null;
        }

        private static string FindEnclosingAddTaskLambda(SyntaxNode node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                if (ancestor is SimpleLambdaExpressionSyntax || ancestor is ParenthesizedLambdaExpressionSyntax)
                {
                    return GetAddTaskMethodName(ancestor);
                }

                // Stop searching if we leave the current method body
                if (ancestor is MethodDeclarationSyntax)
                {
                    return null;
                }
            }

            return null;
        }

        private static string GetAddTaskMethodName(SyntaxNode lambda)
        {
            // Expected tree: Lambda -> Argument -> ArgumentList -> InvocationExpression
            if (lambda.Parent is ArgumentSyntax argument &&
                argument.Parent is ArgumentListSyntax argumentList &&
                argumentList.Parent is InvocationExpressionSyntax outerInvocation)
            {
                string methodName = GetMethodName(outerInvocation);
                if (methodName != null && AddTaskMethodNames.Contains(methodName))
                {
                    return methodName;
                }
            }

            return null;
        }
    }
}
