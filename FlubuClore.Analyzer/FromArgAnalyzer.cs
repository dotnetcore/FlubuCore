using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FlubuClore.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FromArgAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FlubuCore_FromArgUnsuportedPropertyType";

        public const string DianogsticId2 = "FlubuCore_FromArgKeyShoudNotStartWithDash";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString UnsuportedPropertyTypeTitle =
            new LocalizableResourceString(nameof(Resources.FromArgUnsuportedPropertyTypeTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString UnsuportedPropertyTypeMessageFormat =
            new LocalizableResourceString(nameof(Resources.FromArgUnsuportedPropertyTypeMessageFormat),
                Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString UnsuportedPropertyTypeDescription =
            new LocalizableResourceString(nameof(Resources.FirstTargetParameterDescription), Resources.ResourceManager,
                typeof(Resources));

        private const string Category = "FromArg";

        private static string[] SupportedTypes = new string[]
            { "string", "short", "int", "int16", "int32", "int64", "double", "bool", "boolean", "DateTime", "uint", "ulong", "ushort"};

        private static DiagnosticDescriptor FirstParameterMustBeOfTypeITarget = new DiagnosticDescriptor(DiagnosticId,
            UnsuportedPropertyTypeTitle, UnsuportedPropertyTypeMessageFormat, Category, DiagnosticSeverity.Warning,
            isEnabledByDefault: true, description: UnsuportedPropertyTypeDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(FirstParameterMustBeOfTypeITarget); }
        }
        
        public override void Initialize(AnalysisContext context)
        {
           
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Property);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var propertySymbol = (IPropertySymbol)context.Symbol;
            var attributes = propertySymbol.GetAttributes();
           ;
            if (attributes.Length == 0)
            {
                return;
            }

            foreach (var attribute in attributes)
            {
                if (attribute.AttributeClass.Name != "FromArg" && attribute.AttributeClass.Name != "FromArgAttribute")
                {
                    continue;
                }

                if (!SupportedTypes.Contains(propertySymbol.Type.Name, StringComparer.OrdinalIgnoreCase))
                {
                    var attributeSyntax =
                        (AttributeSyntax) attribute.ApplicationSyntaxReference.GetSyntax(
                            context.CancellationToken);
                    var diagnostic = Diagnostic.Create(FirstParameterMustBeOfTypeITarget, attributeSyntax.GetLocation(), propertySymbol.Type.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
