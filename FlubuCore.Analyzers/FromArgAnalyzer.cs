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

        private const string Category = "FromArg";

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

        private static string[] _supportedTypes = new string[]
            { "string", "short", "int", "int16", "int32", "int64", "double", "bool", "boolean", "DateTime", "uint", "ulong", "ushort", "List", "IList", "IEnumerable" };

        private static DiagnosticDescriptor _propertyTypeMustBeSupported = new DiagnosticDescriptor(DiagnosticId,
            UnsuportedPropertyTypeTitle, UnsuportedPropertyTypeMessageFormat, Category, DiagnosticSeverity.Error,
            isEnabledByDefault: true, description: UnsuportedPropertyTypeDescription);

        private static DiagnosticDescriptor _wrongKeyValue = new DiagnosticDescriptor(DianogsticId2,
            KeyShouldNotStartWithDashTitle, KeyShouldNotStartWithDashMessageFormat, Category, DiagnosticSeverity.Error,
            isEnabledByDefault: true, description: KeyShouldNotStartWithDashDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(_propertyTypeMustBeSupported, _wrongKeyValue); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Property);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var propertySymbol = (IPropertySymbol)context.Symbol;
            var attributes = propertySymbol.GetAttributes();
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

                if (!_supportedTypes.Contains(propertySymbol.Type.Name, StringComparer.OrdinalIgnoreCase))
                {
                    var diagnostic = Diagnostic.Create(_propertyTypeMustBeSupported, propertySymbol.Locations[0], propertySymbol.Type.Name);
                    context.ReportDiagnostic(diagnostic);
                }

                var keyValue = attribute.ConstructorArguments[0].Value as string;

                if (keyValue != null && keyValue.StartsWith("-"))
                {
                    var attributeSyntax = (AttributeSyntax)attribute.ApplicationSyntaxReference.GetSyntax(context.CancellationToken);
                    var diagnostic = Diagnostic.Create(_wrongKeyValue, attributeSyntax.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
