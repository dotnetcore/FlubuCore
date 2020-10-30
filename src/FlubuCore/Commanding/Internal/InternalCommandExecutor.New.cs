using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.Infrastructure.Terminal;
using FlubuCore.IO;
using FlubuCore.Scripting;
using FlubuCore.Tasks.Packaging;
using FlubuCore.Templating;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;

namespace FlubuCore.Commanding.Internal
{
    public partial class InternalCommandExecutor
    {
        private const string TmpZipPath = "./template.zip";

        private const string TemplateJsonFileName = "template.json";

        private const string TemplateCsFileName = "template.cs";

        private const string DefaultTemplateUrl = "https://github.com/flubu-core/FlubuCore.DefaultTemplate/archive/master.zip";

        private const string LibraryTemplateUrl = "https://github.com/flubu-core/FlubuCore.LibraryTemplate/archive/master.zip";

        private bool DefaultTemplateCommand => Args.MainCommands.Count == 1 && Args.ScriptArguments.Count == 0;

        private bool LibraryTemplateCommand => Args.MainCommands.Count == 2 &&
                                              (Args.MainCommands[1].Equals("lib", StringComparison.OrdinalIgnoreCase) || Args.MainCommands[1].Equals("library", StringComparison.OrdinalIgnoreCase));

        private bool CustomTemplateCommand => Args.MainCommands.Count == 1 && Args.ScriptArguments.Count > 0 && Args.ScriptArguments.ContainsKey("u");

        internal async Task CreateNewProject()
        {
            if (DefaultTemplateCommand)
            {
                await DownloadAndPrepareProject(DefaultTemplateUrl);
            }
            else if (LibraryTemplateCommand)
            {
                await DownloadAndPrepareProject(LibraryTemplateUrl);
            }
            else if (CustomTemplateCommand)
            {
                if (!TryGetTemplateUri(out var templateUri))
                {
                    FlubuSession.LogInfo("Entered uri is not well formed.");
                    return;
                }

                await DownloadAndPrepareProject(templateUri);
            }
        }

        internal async Task DownloadAndPrepareProject(string templateUri)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    await client.DownloadFileAsync(templateUri, TmpZipPath);
                    var rootDir = Path.GetFullPath(".");

                    var files = FlubuSession.Tasks().UnzipTask(TmpZipPath, rootDir).NoLog().Execute(FlubuSession);

                    if (!files.Any(x => x.EndsWith(".cs")))
                    {
                       FlubuSession.LogInfo("Flubu template not found on specified url.");
                       return;
                    }

                    string templateJsonPath = files.FirstOrDefault(x => x.EndsWith(TemplateJsonFileName, StringComparison.OrdinalIgnoreCase));
                    string templateCsFilePath = files.FirstOrDefault(x => x.EndsWith(TemplateCsFileName, StringComparison.OrdinalIgnoreCase));
                    TemplateModel templateData = null;
                    if (templateJsonPath != null)
                    {
                        templateData = GetTemplateDataFromJsonFile(templateJsonPath);
                    }
                    else if (templateCsFilePath != null)
                    {
                        templateData = await GetTemplateDataFromCsharpFile(templateCsFilePath);

                        if (templateData == null)
                        {
                            FlubuSession.LogInfo("Template.cs must implement IFlubuTemplate interface.");
                        }
                    }

                    var replacementTokens = GetReplacementTokens(templateData);

                    foreach (var sourceFile in files)
                    {
                        string relativePath = sourceFile.Replace(rootDir, string.Empty).TrimStart(Path.DirectorySeparatorChar);

                        var destinationFile = Path.Combine(rootDir, relativePath
                            .Substring(relativePath.IndexOf(Path.DirectorySeparatorChar))
                            .TrimStart(Path.DirectorySeparatorChar));

                        var destinationDir = Path.GetDirectoryName(destinationFile);

                        if (replacementTokens.Any())
                        {
                            FlubuSession.Tasks().ReplaceTokensTask(sourceFile)
                                .Replace(replacementTokens.ToArray()).Execute(FlubuSession);
                        }

                        if (!string.IsNullOrEmpty(destinationDir))
                        {
                            Directory.CreateDirectory(destinationDir);
                        }

                        File.Copy(sourceFile, destinationFile, true);
                    }

                    var tmp = files[0].Substring(rootDir.Length).TrimStart(Path.DirectorySeparatorChar);
                    var gitDirName = tmp.Substring(0, tmp.IndexOf(Path.DirectorySeparatorChar));
                    Directory.Delete(gitDirName, true);
                }
                catch (InvalidDataException)
                {
                    FlubuSession.LogInfo("Flubu template not found on specified url.");
                }
                catch (HttpRequestException)
                {
                    FlubuSession.LogInfo("Flubu template not found on specified url.");
                }
                finally
                {
                    File.Delete(TmpZipPath);
                }
            }
        }

        private static List<Tuple<string, string>> GetReplacementTokens(TemplateModel templateData)
        {
            List<Tuple<string, string>> replacmentTokens = new List<Tuple<string, string>>();

            if (templateData?.Tokens != null && templateData.Tokens.Count > 0)
            {
                foreach (var token in templateData.Tokens)
                {
                    var initialText = !string.IsNullOrEmpty(token.Description)
                        ? token.Description
                        : $"Enter replacement value for {token.Token}";
                    var console = new FlubuConsole(new List<Hint>(), options: o =>
                    {
                        o.WritePrompt = false;
                        o.InitialText = initialText;
                    });

                    var newValue = console.ReadLine();
                    replacmentTokens.Add(new Tuple<string, string>(token.Token, newValue));
                }
            }

            return replacmentTokens;
        }

        private static TemplateModel GetTemplateDataFromJsonFile(string templateJsonPath)
        {
            TemplateModel templateData;
            var json = File.ReadAllText(templateJsonPath);
            templateData = JsonConvert.DeserializeObject<TemplateModel>(json);
            return templateData;
        }

        private async Task<TemplateModel> GetTemplateDataFromCsharpFile(string templateCsFilePath)
        {
            TemplateModel templateData;
            var assemblyInfos = GetAssemblyReferencesForTemplating();
            var assemblyReferencesLocations = assemblyInfos.Select(x => x.FullPath).ToList();
            var references = assemblyReferencesLocations.Select(i => MetadataReference.CreateFromFile(i));
            ScriptOptions opts = ScriptOptions.Default
                .WithEmitDebugInformation(true)
                .WithFilePath(templateCsFilePath)
                .WithFileEncoding(Encoding.UTF8)
                .WithReferences(references);

            var code = File.ReadAllText(templateCsFilePath);
            Script script = CSharpScript
                .Create(string.Join("\r\n", code), opts)
                .ContinueWith("var sc = new Template();");

            ScriptState result = await script.RunAsync();
            IFlubuTemplate flubuTemplate = result.Variables[0].Value as IFlubuTemplate;

            if (flubuTemplate == null)
            {
                return null;
            }

            var templateBuilder = new FlubuTemplateBuilder();
            flubuTemplate.ConfigureTemplate(templateBuilder);
            templateData = templateBuilder.Build();
            return templateData;
        }

        private bool TryGetTemplateUri(out string templateLocation)
        {
            templateLocation = Args.ScriptArguments["u"];
            if (!Uri.IsWellFormedUriString(templateLocation, UriKind.Absolute))
            {
                return false;
            }

            if ((templateLocation.StartsWith("http") || templateLocation.StartsWith("https")) &&
                !templateLocation.EndsWith("zip"))
            {
                templateLocation = $"{templateLocation}/archive/master.zip";
            }

            return true;
        }

         private List<AssemblyInfo> GetAssemblyReferencesForTemplating()
        {
            var coreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var flubuAss = typeof(IFlubuTemplate).GetTypeInfo().Assembly;
            var objAss = typeof(object).GetTypeInfo().Assembly;
            var linqAss = typeof(ILookup<string, string>).GetTypeInfo().Assembly;

            List<AssemblyInfo> assemblyReferenceLocations = new List<AssemblyInfo>
            {
               new AssemblyInfo
               {
                   Name = "mscorlib",
                   FullPath = Path.Combine(coreDir, "mscorlib.dll"),
                   VersionStatus = VersionStatus.Sealed,
               },
               flubuAss.ToAssemblyInfo(),
               objAss.ToAssemblyInfo(),
               linqAss.ToAssemblyInfo(),
            };

            assemblyReferenceLocations.AddReferenceByAssemblyName("System");
            assemblyReferenceLocations.AddReferenceByAssemblyName("System.Collections");

            return assemblyReferenceLocations;
        }
    }
}
