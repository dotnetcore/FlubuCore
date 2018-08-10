using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FlubuClore.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TargetParameterAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FlubuCore_TargetParameterAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString FirstTargetParameterTitle =
            new LocalizableResourceString(nameof(Resources.FirstTargetParameterTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString FirstTargetParameterMessageFormat =
            new LocalizableResourceString(nameof(Resources.FirstTargetParameterMessageFormat),
                Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString FirstTargetParameterDescription =
            new LocalizableResourceString(nameof(Resources.FirstTargetParameterDescription), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString ParameterCountTitle =
            new LocalizableResourceString(nameof(Resources.TargetParameterCountTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString ParameteCountMessageFormat =
            new LocalizableResourceString(nameof(Resources.TargetParameterCountMessageFormat),
                Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString ParameterCountDescription =
            new LocalizableResourceString(nameof(Resources.TargetParameterCountDescription), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString ParameterNotSameTitle =
            new LocalizableResourceString(nameof(Resources.TargetParameterTypeNotSameTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString ParameteNotSameFormat =
            new LocalizableResourceString(nameof(Resources.TargetParameterTypeNotSameMessageFormat),
                Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString ParameterNotSameDescription =
            new LocalizableResourceString(nameof(Resources.TargetParameterTypeNotSameDescritpion),
                Resources.ResourceManager, typeof(Resources));


        private const string Category = "TargetDefinition";

        private static DiagnosticDescriptor FirstParameterMustBeOfTypeITarget = new DiagnosticDescriptor(DiagnosticId,
            FirstTargetParameterTitle, FirstTargetParameterMessageFormat, Category, DiagnosticSeverity.Warning,
            isEnabledByDefault: true, description: FirstTargetParameterDescription);

        private static DiagnosticDescriptor AttributeAndMethodParameterCountMustBeTheSame =
            new DiagnosticDescriptor(DiagnosticId, ParameterCountTitle, ParameteCountMessageFormat, Category,
                DiagnosticSeverity.Warning, isEnabledByDefault: true, description: ParameterCountDescription);

        private static DiagnosticDescriptor AttributeAndMethodParameterTypeMustBeTheSame =
            new DiagnosticDescriptor(DiagnosticId, ParameterNotSameTitle, ParameteNotSameFormat, Category,
                DiagnosticSeverity.Warning, isEnabledByDefault: true, description: ParameterNotSameDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(FirstParameterMustBeOfTypeITarget,
                    AttributeAndMethodParameterCountMustBeTheSame, AttributeAndMethodParameterTypeMustBeTheSame);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;
            var attributes = methodSymbol.GetAttributes();

            foreach (var attribute in attributes)
            {
                ImmutableArray<TypedConstant> attributeParameters;
                bool hasAttributeParameters = false;
                if (attribute.ConstructorArguments.Length > 1)
                {
                    hasAttributeParameters = true;
                    attributeParameters = attribute.ConstructorArguments[1].Values;
                }

                if (attribute.AttributeClass.Name != "Target" && attribute.AttributeClass.Name != "TargetAttribute")
                {
                    continue;
                }

                if (methodSymbol.Parameters.Length == 0)
                {
                    var diagnostic = Diagnostic.Create(FirstParameterMustBeOfTypeITarget, methodSymbol.Locations[0],
                        methodSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
                else if (methodSymbol.Parameters[0].Type.Name != "ITarget")
                {
                    var diagnostic = Diagnostic.Create(FirstParameterMustBeOfTypeITarget, methodSymbol.Locations[0],
                        methodSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }

                else if (hasAttributeParameters && methodSymbol.Parameters.Length != attributeParameters.Length + 1)
                {
                    var diagnostic = Diagnostic.Create(AttributeAndMethodParameterCountMustBeTheSame, methodSymbol.Locations[0], methodSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
                else
                {
                    if (!hasAttributeParameters)
                    {
                        continue;
                    }

                    for (int i = 0; i < attributeParameters.Length; i++)
                    {
                        if (attributeParameters[i].Type.Name != methodSymbol.Parameters[i + 1].Type.Name)
                        {
                            var attributeSyntax =
                                (AttributeSyntax) attribute.ApplicationSyntaxReference.GetSyntax(
                                    context.CancellationToken);
                            var argument = attributeSyntax.ArgumentList.Arguments[i + 1];
                            var diagnostic = Diagnostic.Create(AttributeAndMethodParameterTypeMustBeTheSame,
                                argument.GetLocation(), methodSymbol.Name, methodSymbol.Parameters[i + 1].Name);
                            context.ReportDiagnostic(diagnostic);
                        }
                    }

                }
            }
        }
    }
}
