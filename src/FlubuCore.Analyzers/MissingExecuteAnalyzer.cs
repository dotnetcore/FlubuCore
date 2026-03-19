using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FlubuCore.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingExecuteAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FlubuCore_Execute_002";

        private const string Category = "TaskExecution";

        private static readonly LocalizableString Title = "Missing Execute call on task";

        private static readonly string MessageFormat =
            "Task created via '{0}' is never executed. Call Execute(context) or ExecuteAsync(context) on the task.";

        private static readonly string Description =
            "Tasks created via context.Tasks() or context.CoreTasks() in Do methods must be explicitly executed.";

        private static readonly DiagnosticDescriptor _missingExecute = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
            isEnabledByDefault: true, description: Description);

        private static readonly HashSet<string> ExecuteMethodNames = new HashSet<string>
        {
            "Execute",
            "ExecuteAsync",
            "ExecuteVoid",
            "ExecuteVoidAsync"
        };

        private static readonly HashSet<string> TaskFluentFactoryNames = new HashSet<string>
        {
            "Tasks",
            "CoreTasks"
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(_missingExecute);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            if (!IsTaskFactoryCall(invocation))
            {
                return;
            }

            string factoryMethodName = GetMethodName(invocation);

            if (ChainEndsWithExecute(invocation))
            {
                return;
            }

            string variableName = GetAssignedVariableName(invocation);
            if (variableName != null)
            {
                var methodBody = GetContainingMethodBody(invocation);
                if (methodBody != null && HasExecuteOnVariable(methodBody, variableName))
                {
                    return;
                }
            }

            var topExpression = GetTopOfChain(invocation);
            var diagnostic = Diagnostic.Create(_missingExecute, topExpression.GetLocation(), factoryMethodName);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsTaskFactoryCall(InvocationExpressionSyntax invocation)
        {
            // Matches: context.Tasks().Something() or context.CoreTasks().Something()
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Expression is InvocationExpressionSyntax innerInvocation)
            {
                string innerMethodName = GetMethodName(innerInvocation);
                return innerMethodName != null && TaskFluentFactoryNames.Contains(innerMethodName);
            }

            return false;
        }

        private static bool ChainEndsWithExecute(InvocationExpressionSyntax invocation)
        {
            // Walk UP the fluent chain from the task factory call
            // Check if any method in the chain above is Execute/ExecuteAsync
            SyntaxNode current = invocation;
            while (current.Parent is MemberAccessExpressionSyntax parentMemberAccess &&
                   parentMemberAccess.Parent is InvocationExpressionSyntax parentInvocation)
            {
                if (ExecuteMethodNames.Contains(parentMemberAccess.Name.Identifier.Text))
                {
                    return true;
                }

                current = parentInvocation;
            }

            return false;
        }

        private static SyntaxNode GetTopOfChain(InvocationExpressionSyntax invocation)
        {
            SyntaxNode current = invocation;
            while (current.Parent is MemberAccessExpressionSyntax ma &&
                   ma.Parent is InvocationExpressionSyntax parentInv)
            {
                current = parentInv;
            }

            return current;
        }

        private static string GetAssignedVariableName(InvocationExpressionSyntax invocation)
        {
            var top = GetTopOfChain(invocation);
            if (top.Parent is EqualsValueClauseSyntax equalsValue &&
                equalsValue.Parent is VariableDeclaratorSyntax declarator)
            {
                return declarator.Identifier.Text;
            }

            // Also handle reassignment: task = context.Tasks().RunProgramTask(...)
            if (top.Parent is AssignmentExpressionSyntax assignment &&
                assignment.Left is IdentifierNameSyntax identifier)
            {
                return identifier.Identifier.Text;
            }

            return null;
        }

        private static SyntaxNode GetContainingMethodBody(SyntaxNode node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                if (ancestor is MethodDeclarationSyntax method)
                {
                    return (SyntaxNode)method.Body ?? method.ExpressionBody;
                }

                if (ancestor is LocalFunctionStatementSyntax localFunc)
                {
                    return (SyntaxNode)localFunc.Body ?? localFunc.ExpressionBody;
                }
            }

            return null;
        }

        private static bool HasExecuteOnVariable(SyntaxNode methodBody, string variableName)
        {
            foreach (var invocation in methodBody.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                string methodName = GetMethodName(invocation);
                if (methodName != null && ExecuteMethodNames.Contains(methodName))
                {
                    if (ExpressionStartsWithIdentifier(invocation.Expression, variableName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool ExpressionStartsWithIdentifier(ExpressionSyntax expr, string name)
        {
            while (true)
            {
                if (expr is MemberAccessExpressionSyntax ma)
                {
                    expr = ma.Expression;
                }
                else if (expr is InvocationExpressionSyntax inv)
                {
                    expr = inv.Expression;
                }
                else
                {
                    break;
                }
            }

            return expr is IdentifierNameSyntax id && id.Identifier.Text == name;
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
    }
}
