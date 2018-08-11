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
        public const string DiagnosticId = "FlubuCore_FromArg_001";

        public const string DianogsticId2 = "FlubuCore_FromArg_002";

        private static readonly LocalizableString UnsuportedPropertyTypeTitle = "Unsuported property type";


        private static readonly LocalizableString UnsuportedPropertyTypeMessageFormat =
            "FromArg does not support type '{0}' on property.";

        private static readonly LocalizableString UnsuportedPropertyTypeDescription =
            "FromArg does not support specificied type on property. ";

        private static readonly LocalizableString KeyShouldNotStartWithDashTitle =
            "Wrong key value";

        private static readonly LocalizableString KeyShouldNotStartWithDashMessageFormat =
            "Key should not start with dash.";

        private static readonly LocalizableString KeyShouldNotStartWithDashDescription =
            "Key should not start with dash.";

        private const string Category = "FromArg";

        private static string[] SupportedTypes = new string[]
            { "string", "short", "int", "int16", "int32", "int64", "double", "bool", "boolean", "DateTime", "uint", "ulong", "ushort", "List", "IList", "IEnumerable"};

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
                    var attributeSyntax = (AttributeSyntax)attribute.ApplicationSyntaxReference.GetSyntax(context.CancellationToken);
                    var diagnostic = Diagnostic.Create(PropertyTypeMustBeSupported, propertySymbol.Locations[0], propertySymbol.Type.Name);
                    context.ReportDiagnostic(diagnostic);
                }

                var keyValue = attribute.ConstructorArguments[0].Value as string;

                if (keyValue != null && keyValue.StartsWith("-"))
                {
                    var attributeSyntax = (AttributeSyntax)attribute.ApplicationSyntaxReference.GetSyntax(context.CancellationToken);
                    var diagnostic = Diagnostic.Create(WrongKeyValue, attributeSyntax.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
