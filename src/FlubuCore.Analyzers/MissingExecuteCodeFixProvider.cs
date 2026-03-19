using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FlubuCore.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingExecuteCodeFixProvider))]
    [Shared]
    public class MissingExecuteCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Add Execute(context) call";

        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(MissingExecuteAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() =>
            WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindNode(diagnosticSpan);
            var invocation = node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (invocation == null)
            {
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => AddExecuteCallAsync(context.Document, invocation, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private static async Task<Document> AddExecuteCallAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            // Find the top of the fluent chain
            var topInvocation = GetTopOfChain(invocation);

            // Find the context parameter name from the containing method
            var contextParamName = FindContextParameterName(topInvocation);

            // Build: existingChain.Execute(context)
            var executeAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                topInvocation.WithoutTrailingTrivia(),
                SyntaxFactory.IdentifierName("Execute"));

            var contextArgument = SyntaxFactory.Argument(
                SyntaxFactory.IdentifierName(contextParamName ?? "context"));

            var executeInvocation = SyntaxFactory.InvocationExpression(
                executeAccess,
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(contextArgument)))
                .WithTrailingTrivia(topInvocation.GetTrailingTrivia());

            var newRoot = root.ReplaceNode(topInvocation, executeInvocation);
            return document.WithSyntaxRoot(newRoot);
        }

        private static InvocationExpressionSyntax GetTopOfChain(InvocationExpressionSyntax invocation)
        {
            var current = (SyntaxNode)invocation;
            while (current.Parent is MemberAccessExpressionSyntax ma &&
                   ma.Parent is InvocationExpressionSyntax parentInv)
            {
                current = parentInv;
            }

            return (InvocationExpressionSyntax)current;
        }

        private static string FindContextParameterName(SyntaxNode node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                if (ancestor is MethodDeclarationSyntax method)
                {
                    // Look for a parameter whose type name contains "Context"
                    foreach (var param in method.ParameterList.Parameters)
                    {
                        var typeName = param.Type?.ToString() ?? string.Empty;
                        if (typeName.Contains("Context"))
                        {
                            return param.Identifier.Text;
                        }
                    }

                    break;
                }
            }

            return null;
        }
    }
}
