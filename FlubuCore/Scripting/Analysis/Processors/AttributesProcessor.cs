using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting.Attributes;

namespace FlubuCore.Scripting.Analysis.Processors
{
    public class AttributesProcessor : IScriptProcessor
    {
        private readonly IFileWrapper _file;

        private readonly IPathWrapper _pathWrapper;

        public AttributesProcessor(IFileWrapper file, IPathWrapper pathWrapper)
        {
            _file = file;
            _pathWrapper = pathWrapper;
        }

        public bool Process(ScriptAnalyzerResult analyzerResult, string line, int lineIndex)
        {
            if (!line.StartsWith("["))
            {
                return false;
            }

            var endAttributeIndex = line.IndexOf(']');

            if (endAttributeIndex < 0)
            {
                return false;
            }

            var attributeName = line.Substring(1, endAttributeIndex - 1);
            if (attributeName.StartsWith("Assembly"))
            {
                ProcessAssemblyAttribute(analyzerResult, line);
            }
            else if (attributeName.StartsWith("NugetPackage"))
            {
                ProcessNugetPackageAttribute(analyzerResult, line);
            }
            else if (attributeName.StartsWith("Reference"))
            {
                ProcessReferenceAttribute(analyzerResult, line);
            }
            else if (attributeName == nameof(DisableLoadScriptReferencesAutomaticallyAttribute).Replace("Attribute", string.Empty) ||
                attributeName == nameof(DisableLoadScriptReferencesAutomaticallyAttribute))
            {
                analyzerResult.ScriptAttributes.Add(ScriptConfigAttributes.DisableLoadScriptReferencesAutomatically);
            }
            else if (attributeName == nameof(AlwaysRecompileScriptAttribute).Replace("Attribute", string.Empty) ||
                attributeName == nameof(AlwaysRecompileScriptAttribute))
            {
                analyzerResult.ScriptAttributes.Add(ScriptConfigAttributes.AlwaysRecompileScript);
            }
            else if (attributeName == nameof(CreateBuildScriptInstanceOldWayAttribute).Replace("Attribute", string.Empty) ||
                     attributeName == nameof(CreateBuildScriptInstanceOldWayAttribute))
            {
                analyzerResult.ScriptAttributes.Add(ScriptConfigAttributes.CreateBuildScriptInstanceOldWayAttribute);
            }

            return true;
        }

        private static void ProcessReferenceAttribute(ScriptAnalyzerResult analyzerResult, string line)
        {
            int startParametersIndex = line.IndexOf('(') + 2;
            int endParameterIndex = line.IndexOf(')') - 1;
            string reference = line.Substring(startParametersIndex, endParameterIndex - startParametersIndex);
            var type = Type.GetType(reference, true);
            var ass = type.GetTypeInfo().Assembly;
            analyzerResult.AssemblyReferences.Add(ass.ToAssemblyInfo());
        }

        private void ProcessAssemblyAttribute(ScriptAnalyzerResult analyzerResult, string line)
        {
            int startParametersIndex = line.IndexOf('(') + 2;
            int endParameterIndex = line.IndexOf(')') - 1;
            string dll = line.Substring(startParametersIndex, endParameterIndex - startParametersIndex);
            string pathToDll = Path.GetFullPath(dll.Trim());
            string extension = _pathWrapper.GetExtension(pathToDll);
            if (!extension.Equals(".dll", StringComparison.OrdinalIgnoreCase))
            {
                if (!extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ScriptException($"File doesn't have dll extension. {pathToDll}");
                }
            }

            if (!_file.Exists(pathToDll))
            {
                throw new ScriptException($"Assembly not found at location: {pathToDll}");
            }

            analyzerResult.AssemblyReferences.Add(new AssemblyInfo
            {
                Name = "N/A",
                VersionStatus = VersionStatus.NotAvailable,
                FullPath = pathToDll
            });
        }

        private void ProcessNugetPackageAttribute(ScriptAnalyzerResult analyzerResult, string line)
        {
            int startParametersIndex = line.IndexOf('(') + 2;
            int endParameterIndex = line.IndexOf(')') - 1;
            string nugetPackage = line.Substring(startParametersIndex, endParameterIndex - startParametersIndex);
            var nugetInfo = nugetPackage.Split(',');

            analyzerResult.NugetPackageReferences.Add(new NugetPackageReference { Id = nugetInfo[0].Replace("\"", string.Empty).Trim(), Version = nugetInfo[1].Replace("\"", string.Empty).Trim() });
        }
    }
}
