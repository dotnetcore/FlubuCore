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
    
        private static readonly LocalizableString UnsuportedPropertyTypeTitle =
            new LocalizableResourceString(nameof(Resources.FromArgUnsuportedPropertyTypeTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString UnsuportedPropertyTypeMessageFormat =
            new LocalizableResourceString(nameof(Resources.FromArgUnsuportedPropertyTypeMessageFormat),
                Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString UnsuportedPropertyTypeDescription =
            new LocalizableResourceString(nameof(Resources.FirstTargetParameterDescription), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString KeyShouldNotStartWithDashTitle =
            new LocalizableResourceString(nameof(Resources.KeyShouldNotStartWithDashTitle), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString KeyShouldNotStartWithDashMessageFormat =
            new LocalizableResourceString(nameof(Resources.KeyShouldNotStartWithDashMessageFormat),
                Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString KeyShouldNotStartWithDashDescription =
            new LocalizableResourceString(nameof(Resources.KeyShouldNotStartWithDashDescription), Resources.ResourceManager,
                typeof(Resources));

        private const string Category = "FromArg";

        private static string[] SupportedTypes = new string[]
            { "string", "short", "int", "int16", "int32", "int64", "double", "bool", "boolean", "DateTime", "uint", "ulong", "ushort"};

        private static DiagnosticDescriptor PropertyTypeMustBeSupported = new DiagnosticDescriptor(DiagnosticId,
            UnsuportedPropertyTypeTitle, UnsuportedPropertyTypeMessageFormat, Category, DiagnosticSeverity.Warning,
            isEnabledByDefault: true, description: UnsuportedPropertyTypeDescription);

        private static DiagnosticDescriptor WrongKeyValue = new DiagnosticDescriptor(DianogsticId2,
            KeyShouldNotStartWithDashTitle, KeyShouldNotStartWithDashMessageFormat, Category, DiagnosticSeverity.Warning,
            isEnabledByDefault: true, description: KeyShouldNotStartWithDashDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(PropertyTypeMustBeSupported, WrongKeyValue); }
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
                    var attributeSyntax = (AttributeSyntax) attribute.ApplicationSyntaxReference.GetSyntax(context.CancellationToken);
                    var diagnostic = Diagnostic.Create(PropertyTypeMustBeSupported, attributeSyntax.GetLocation(), propertySymbol.Type.Name);
                    context.ReportDiagnostic(diagnostic);
                }

                var keyValue = attribute.ConstructorArguments[0].Value as string;

                if (keyValue != null && keyValue.StartsWith("-"))
                {
                    var attributeSyntax = (AttributeSyntax) attribute.ApplicationSyntaxReference.GetSyntax(context.CancellationToken);
                    var diagnostic = Diagnostic.Create(WrongKeyValue, attributeSyntax.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
