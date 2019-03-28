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

        private readonly IDirectoryWrapper _directory;

        public AttributesProcessor(IFileWrapper file, IPathWrapper pathWrapper, IDirectoryWrapper directory)
        {
            _file = file;
            _pathWrapper = pathWrapper;
            _directory = directory;
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
            if (attributeName.StartsWith("NugetPackage"))
            {
                ProcessNugetPackageAttribute(analyzerResult, line);
            }
            else if (attributeName.StartsWith("AssemblyFromDirectory"))
            {
                ProcessAssemblyFromDirectoryAttribute(analyzerResult, line);
            }
            else if (attributeName.StartsWith("Assembly"))
            {
                ProcessAssemblyAttribute(analyzerResult, line);
            }
            else if (attributeName.StartsWith("IncludeFromDirectory"))
            {
                ProcessIncludeFromDirectoryAttribute(analyzerResult, line);
            }
            else if (attributeName.StartsWith("Include"))
            {
                ProcessIncludeAttribute(analyzerResult, line);
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

        private static void ProcessIncludeFromDirectoryAttribute(ScriptAnalyzerResult analyzerResult, string line)
        {
            var parameters = GetMultipleParametersFromAttribute(line);

            bool includeSubDirectories = false;

            if (parameters.Length > 1)
            {
                includeSubDirectories = bool.Parse(parameters[1]);
            }

            analyzerResult.CsDirectories.Add(new Tuple<(string path, bool includeSubDirectories)>((Path.GetFullPath(parameters[0].Replace("\"", string.Empty)), includeSubDirectories)));
        }

        private static void ProcessIncludeAttribute(ScriptAnalyzerResult analyzerResult, string line)
        {
            var file = GetSingleParameterFromAttrbiute(line);
            analyzerResult.CsFiles.Add(Path.GetFullPath(file.Trim()));
        }

        private static void ProcessReferenceAttribute(ScriptAnalyzerResult analyzerResult, string line)
        {
            var reference = GetSingleParameterFromAttrbiute(line);
            var type = Type.GetType(reference, true);
            var ass = type.GetTypeInfo().Assembly;
            analyzerResult.AssemblyReferences.Add(ass.ToAssemblyInfo());
        }

        private static void ProcessNugetPackageAttribute(ScriptAnalyzerResult analyzerResult, string line)
        {
            var nugetInfo = GetMultipleParametersFromAttribute(line);

            analyzerResult.NugetPackageReferences.Add(new NugetPackageReference { Id = nugetInfo[0].Replace("\"", string.Empty).Trim(), Version = nugetInfo[1].Replace("\"", string.Empty).Trim() });
        }

        private static string[] GetMultipleParametersFromAttribute(string line)
        {
            int startParametersIndex = line.IndexOf('\"') + 1;
            int endParameterIndex = line.IndexOf(')');
            string parameters = line.Substring(startParametersIndex, endParameterIndex - startParametersIndex);
            var splitedParameters = parameters.Split(',');
            return splitedParameters;
        }

        private static string GetSingleParameterFromAttrbiute(string line)
        {
            int startParametersIndex = line.IndexOf('\"') + 1;
            int endParameterIndex = line.IndexOf(')') - 1;
            string reference = line.Substring(startParametersIndex, endParameterIndex - startParametersIndex)
                .Replace("\"", string.Empty).Trim();
            return reference;
        }

        private void ProcessAssemblyAttribute(ScriptAnalyzerResult analyzerResult, string line)
        {
            string dll = GetSingleParameterFromAttrbiute(line);
            string pathToDll = Path.GetFullPath(dll);
            ProcessAssembly(analyzerResult, pathToDll);
        }

        private void ProcessAssemblyFromDirectoryAttribute(ScriptAnalyzerResult analyzerResult, string line)
        {
            var parameters = GetMultipleParametersFromAttribute(line);

            bool includeSubDirectories = false;

            if (parameters.Length > 1)
            {
                includeSubDirectories = bool.Parse(parameters[1]);
            }

            var directory = Path.GetFullPath(parameters[0].Replace("\"", string.Empty));
            var assemblies = _directory.GetFiles(directory, "*.dll", includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (var assembly in assemblies)
            {
                ProcessAssembly(analyzerResult, assembly);
            }
        }

        private void ProcessAssembly(ScriptAnalyzerResult analyzerResult, string pathToDll)
        {
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
                Name = pathToDll,
                VersionStatus = VersionStatus.NotAvailable,
                FullPath = pathToDll
            });
        }
    }
}
