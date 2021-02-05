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
    public class FlubuAttributesWrongTypeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FlubuCore_BuildPropertyAttribute_001";

        private const string Category = "WrongType";

        private static readonly LocalizableString WrongPropertyTypeTitle = "Wrong property type";

        private static readonly LocalizableString WrongPropertyTypeMessageFormat =
            "Property type in combination with attribute '{0}' should be of type '{1}'.";

        private static readonly LocalizableString WrongPropertyTypeDescription =
            "Wrong property type in combination with attribute.";

        private static DiagnosticDescriptor _wrongPropertyType = new DiagnosticDescriptor(DiagnosticId,
            WrongPropertyTypeTitle, WrongPropertyTypeMessageFormat, Category, DiagnosticSeverity.Error,
            isEnabledByDefault: true, description: WrongPropertyTypeDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(_wrongPropertyType); }
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
                switch (attribute.AttributeClass.Name)
                {
                    case "ProductId":
                    case "ProductIdAttribute":
                    case "SolutionFileName":
                    case "SolutionFileNameAttribute":
                    case "BuildConfiguration":
                    case "BuildConfigurationAttribute":
                    case "BuildDir":
                    case "BuildDirAttribute":
                    {
                        var diagnostic = Diagnostic.Create(_wrongPropertyType, propertySymbol.Locations[0], attribute.AttributeClass.Name, "string");
                        context.ReportDiagnostic(diagnostic);
                    }

                    break;
                    case "BuildVersion":
                    case "BuildVersionAttribute":
                    case "GitVersion":
                    case "GitVersionAttribute":
                    case "FetchBuildVersionFromFile":
                    case "FetchBuildVersionFromFileAttribute":
                    {
                        var diagnostic = Diagnostic.Create(_wrongPropertyType, propertySymbol.Locations[0], attribute.AttributeClass.Name, "BuildVersion");
                        context.ReportDiagnostic(diagnostic);
                    }

                    break;
                    case "VSSolution":
                    case "VSSolutionAttribute":
                    case "LoadSolutionAttribute":
                    case "LoadSolution":
                    {
                        var diagnostic = Diagnostic.Create(_wrongPropertyType, propertySymbol.Locations[0], attribute.AttributeClass.Name, "VSSolution");
                        context.ReportDiagnostic(diagnostic);
                    }

                        break;
                }
            }
        }
    }
}
