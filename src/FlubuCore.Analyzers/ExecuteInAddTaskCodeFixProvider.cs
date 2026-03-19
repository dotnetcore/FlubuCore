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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExecuteInAddTaskCodeFixProvider))]
    [Shared]
    public class ExecuteInAddTaskCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Remove Execute call";

        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(ExecuteInAddTaskAnalyzer.DiagnosticId);

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

            // Only fix if the invocation is a member access like .Execute(...)
            if (!(invocation.Expression is MemberAccessExpressionSyntax memberAccess))
            {
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => RemoveExecuteCallAsync(context.Document, invocation, memberAccess, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private static async Task<Document> RemoveExecuteCallAsync(
            Document document,
            InvocationExpressionSyntax executeInvocation,
            MemberAccessExpressionSyntax memberAccess,
            CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            // Replace task.Execute(context) with just task
            // memberAccess.Expression is the part before .Execute
            var replacement = memberAccess.Expression.WithTriviaFrom(executeInvocation);
            var newRoot = root.ReplaceNode(executeInvocation, replacement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
