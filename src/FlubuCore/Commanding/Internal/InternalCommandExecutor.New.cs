using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Infrastructure;
using FlubuCore.Infrastructure.Terminal;
using FlubuCore.Scripting;
using FlubuCore.Tasks;
using FlubuCore.Templating;
using FlubuCore.Templating.Models;
using FlubuCore.Templating.Tasks;
using GlobExpressions;
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

        private const string EmptyTemplateUrl = "https://github.com/flubu-core/FlubuCore.DefaultTemplate/archive/master.zip";

        private const string LibraryTemplateUrl = "https://github.com/flubu-core/FlubuCore.LibraryTemplate/archive/master.zip";

        private readonly List<string> _templateTasksToExecute = new List<string>()
        {
            FlubuTemplateTaskName.ReplacementTokenTask
        };

        private bool HelpTemplateCommand => Args.MainCommands.Count == 1 && Args.ScriptArguments.Count == 0;

        private bool EmptyTemplateCommand => Args.MainCommands.Count == 2 &&
                                               (Args.MainCommands[1].Equals("empty", StringComparison.OrdinalIgnoreCase) || Args.MainCommands[1].Equals("library", StringComparison.OrdinalIgnoreCase));

        private bool LibraryTemplateCommand => Args.MainCommands.Count == 2 &&
                                              (Args.MainCommands[1].Equals("lib", StringComparison.OrdinalIgnoreCase) || Args.MainCommands[1].Equals("library", StringComparison.OrdinalIgnoreCase));

        private bool CustomTemplateCommand => Args.MainCommands.Count == 1 && Args.ScriptArguments.Count > 0 && Args.ScriptArguments.ContainsKey("u");

        internal async Task CreateNewProject()
        {
            if (HelpTemplateCommand)
            {
                ShowHelp();
            }
            else if (EmptyTemplateCommand)
            {
                await DownloadAndPrepareProject(EmptyTemplateUrl);
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
                    FlubuSession.LogInfo($"Creating Flubu template '{Args.MainCommands[1]}'.");
                    await client.DownloadFileAsync(templateUri, TmpZipPath);
                    var rootDir = Path.GetFullPath(".");

                    var files = FlubuSession.Tasks()
                        .UnzipTask(TmpZipPath, rootDir)
                        .WithLogLevel(LogLevel.None)
                        .DoNotLogTaskExecutionInfo()
                        .Execute(FlubuSession);

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
                        var flubuTemplate = await GetTemplateFromCsharpFile(templateCsFilePath);
                        switch (flubuTemplate)
                        {
                            case null:
                                FlubuSession.LogInfo("Template.cs must implement IFlubuTemplate interface.");
                                return;
                            //// ReSharper disable once SuspiciousTypeConversion.Global
                            case IFlubuTemplateTask flubuTask:
                                FlubuTemplateTasksExecutor.AddTaskToExecute(flubuTask);
                                break;
                        }

                        var templateBuilder = new FlubuTemplateBuilder();
                        flubuTemplate.ConfigureTemplate(templateBuilder);
                        templateData = templateBuilder.Build();
                    }

                    FlubuTemplateTasksExecutor.SetTasksToExecute(_templateTasksToExecute);

                    FlubuTemplateTasksExecutor.BeforeFileProcessing(templateData, files);

                    foreach (var sourceFilePath in files)
                    {
                        string relativePath = sourceFilePath.Replace(rootDir, string.Empty).TrimStart(Path.DirectorySeparatorChar);

                        relativePath = relativePath
                             .Substring(relativePath.IndexOf(Path.DirectorySeparatorChar))
                             .TrimStart(Path.DirectorySeparatorChar);

                        if (templateData != null && templateData.SkipFiles.Contains(relativePath))
                        {
                            continue;
                        }

                        var destinationFilePath = Path.Combine(rootDir, relativePath);

                        var destinationDir = Path.GetDirectoryName(destinationFilePath);

                        if (!string.IsNullOrEmpty(destinationDir))
                        {
                            Directory.CreateDirectory(destinationDir);
                        }

                        FlubuTemplateTasksExecutor.BeforeFileCopy(sourceFilePath);

                        File.Copy(sourceFilePath, destinationFilePath, true);
                        FlubuTemplateTasksExecutor.AfterFileCopy(destinationFilePath);
                    }

                    FlubuTemplateTasksExecutor.AfterFileProcessing(templateData);

                    var tmp = files[0].Substring(rootDir.Length).TrimStart(Path.DirectorySeparatorChar);
                    var gitDirName = tmp.Substring(0, tmp.IndexOf(Path.DirectorySeparatorChar));
                    Directory.Delete(gitDirName, true);
                    FlubuSession.LogInfo($"The template '{Args.MainCommands[1]}' was created successfully.");
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

        private static void ShowHelp()
        {
            Console.WriteLine("{0, -25}{1, -75}{2, -60}", "Template", "Description", "Template url");
            Console.WriteLine(string.Empty);
            Console.WriteLine("{0, -25}{1, -75}{2, -60}", "empty", "Default empty template with only build target", "https://github.com/flubu-core/FlubuCore.DefaultTemplate");
            Console.WriteLine("{0, -25}{1, -75}{2, -60}", "lib", "Template for libraries. Includes build, pack, test, nuget push targets", "https://github.com/flubu-core/FlubuCore.LibraryTemplate");
            Console.WriteLine("{0, -25}{1, -75}{2, -60}", "-u={CustomTemplateUrl}", "Custom template. provide github, gitlab repo url", "Your custom template url");
            Console.WriteLine(string.Empty);
            Console.WriteLine("Examples:");
            Console.WriteLine("    flubu new lib");
            Console.WriteLine("    flubu new -u=https://github.com/flubu-core/FlubuCore.CustomTemplate");
            Console.WriteLine("    flubu new -u=https://github.com/flubu-core-private/FlubuCore.PrivateTemplate -t={enterAccessToken}");
            Console.WriteLine("    flubu new -u=https://gitlab.com/flubu-core/FlubuCore.CustomTemplate/-/archive/master/FlubuCore.CustomTemplate-master.zip");
            Console.WriteLine("    flubu new -u=https://gitlab.com/flubu-core-private/FlubuCore.PrivateTemplate/-/archive/master/FlubuCore.PrivateTemplate-master.zip?private_token={enterAccessToken}");
        }

        private TemplateModel GetTemplateDataFromJsonFile(string templateJsonPath)
        {
            var json = File.ReadAllText(templateJsonPath);
            return JsonConvert.DeserializeObject<TemplateModel>(json);
        }

        private async Task<IFlubuTemplate> GetTemplateFromCsharpFile(string templateCsFilePath)
        {
            var assemblyInfos = ScriptLoader.GetDefaultReferences();
            var assemblyReferencesLocations = assemblyInfos.Select(x => x.FullPath).ToList();
            var flubuCoreAssembly = typeof(DefaultBuildScript).GetTypeInfo().Assembly;

            // Enumerate all assemblies referenced by FlubuCore
            // and provide them as references to the build script we're about to
            // compile.
            AssemblyName[] flubuReferencedAssemblies = flubuCoreAssembly.GetReferencedAssemblies();
            foreach (var referencedAssembly in flubuReferencedAssemblies)
            {
                Assembly loadedAssembly = Assembly.Load(referencedAssembly);
                if (string.IsNullOrEmpty(loadedAssembly.Location))
                    continue;

                assemblyInfos.AddOrUpdateAssemblyInfo(new AssemblyInfo
                {
                    Name = referencedAssembly.Name,
                    Version = referencedAssembly.Version,
                    FullPath = loadedAssembly.Location,
                });
            }

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

            return flubuTemplate;
        }

        private bool TryGetTemplateUri(out string templateLocation)
        {
            templateLocation = Args.ScriptArguments["u"];
            if (!Uri.IsWellFormedUriString(templateLocation, UriKind.Absolute))
            {
                return false;
            }

            if ((templateLocation.StartsWith("http") || templateLocation.StartsWith("https")) &&
                !templateLocation.EndsWith("zip") && !templateLocation.Contains("?"))
            {
                templateLocation = $"{templateLocation}/archive/master.zip";
                var token = Args.ScriptArguments["t"];
                if (!string.IsNullOrEmpty(token))
                {
                    templateLocation = $"{templateLocation}?token={token}";
                }
            }

            return true;
        }
    }
}
