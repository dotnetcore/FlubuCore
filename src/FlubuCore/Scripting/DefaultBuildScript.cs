using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FlubuCore.BuildServers.Configurations;
using FlubuCore.BuildServers.Configurations.Models;
using FlubuCore.BuildServers.Configurations.Models.AppVeyor;
using FlubuCore.BuildServers.Configurations.Models.AzurePipelines;
using FlubuCore.BuildServers.Configurations.Models.GitHubActions;
using FlubuCore.BuildServers.Configurations.Models.Jenkins;
using FlubuCore.BuildServers.Configurations.Models.Travis;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.IO;
using FlubuCore.Targeting;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Versioning;
using FlubuCore.WebApi.Client;
using Microsoft.Build.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Scripting
{
    public abstract class DefaultBuildScript : IBuildScript
    {
        private IFlubuSession _flubuSession;

        private IScriptProperties _scriptProperties;

        private ITargetCreator _targetCreator;

        private FlubuConfiguration _flubuConfiguration;

        /// <summary>
        /// Get's product root directory.
        /// </summary>
        protected FullPath RootDirectory => new FullPath(_flubuSession.Properties.GetProductRootDir());

        public int Run(IFlubuSession flubuSession)
        {
            _flubuSession = flubuSession;
            _scriptProperties = flubuSession.ScriptServiceProvider.GetScriptProperties();
            _targetCreator = flubuSession.ScriptServiceProvider.GetTargetCreator();
            _flubuConfiguration = flubuSession.ScriptServiceProvider.GetFlubuConfiguration();

            try
            {
                ConfigureDefaultProps(flubuSession);
                BeforeBuildExecution(flubuSession);
                RunBuild(flubuSession);
                flubuSession.Complete();
                AfterBuildExecution(flubuSession);
                return 0;
            }
            catch (TargetNotFoundException e)
            {
                flubuSession.ResetDepth();
                OnBuildFailed(flubuSession, e);
                AfterBuildExecution(flubuSession);
                if (flubuSession.Args.RethrowOnException)
                    throw;

                flubuSession.LogError($"{Environment.NewLine}{e.Message}");
                return 3;
            }
            catch (WebApiException e)
            {
                flubuSession.ResetDepth();
                OnBuildFailed(flubuSession, e);
                AfterBuildExecution(flubuSession);
                if (flubuSession.Args.RethrowOnException)
                    throw;

                return 1;
            }
            catch (FlubuException e)
            {
                flubuSession.ResetDepth();
                OnBuildFailed(flubuSession, e);

                if (!flubuSession.Args.RethrowOnException)
                {
                    flubuSession.LogError($"ERROR: {e.Message}", Color.Red);
                }

                AfterBuildExecution(flubuSession);
                if (flubuSession.Args.RethrowOnException)
                    throw;

                return 1;
            }
            catch (Exception e)
            {
                flubuSession.ResetDepth();
                OnBuildFailed(flubuSession, e);

                if (!flubuSession.Args.RethrowOnException)
                {
                    flubuSession.LogError($"ERROR: {e}", Color.Red);
                }

                AfterBuildExecution(flubuSession);
                if (flubuSession.Args.RethrowOnException)
                    throw;

                return 2;
            }
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void Configure(IFlubuConfigurationBuilder configurationBuilder, ILoggerFactory loggerFactory)
        {
        }

        protected abstract void ConfigureTargets(ITaskContext context);

        protected virtual void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        public static (List<string> targetsToRun, bool unknownTarget, List<string> notFoundTargets) ParseCmdLineArgs(List<string> mainCommands, TargetTree targetTree)
        {
            if (mainCommands == null || mainCommands.Count == 0) return (null, false, null);

            if (targetTree.HasAllTargets(mainCommands, out var notFoundTargets))
                return (mainCommands, false, null);

            return (new List<string> { FlubuTargets.Help }, true, notFoundTargets);
        }

        private void RunBuild(IFlubuSession flubuSession)
        {
             flubuSession.TargetTree.ResetTargetTree();
            ConfigureBuildProperties(flubuSession);

            ConfigureDefaultTargets(flubuSession);

            _scriptProperties.InjectProperties(this, flubuSession);

            _targetCreator.CreateTargetFromMethodAttributes(this, flubuSession);

            ConfigureTargets(flubuSession);

            if (!flubuSession.Args.InteractiveMode)
            {
                var targetsInfo = ParseCmdLineArgs(flubuSession.Args.MainCommands, flubuSession.TargetTree);
                flubuSession.UnknownTarget = targetsInfo.unknownTarget;
                if (targetsInfo.targetsToRun == null || targetsInfo.targetsToRun.Count == 0)
                {
                    var defaultTargets = flubuSession.TargetTree.DefaultTargets;
                    targetsInfo.targetsToRun = new List<string>();
                    if (defaultTargets != null && defaultTargets.Count != 0)
                    {
                        foreach (var defaultTarget in defaultTargets)
                        {
                            targetsInfo.targetsToRun.Add(defaultTarget.TargetName);
                        }
                    }
                    else
                    {
                        targetsInfo.targetsToRun.Add(FlubuTargets.Help);
                    }
                }

                var generateCIConfigs = flubuSession.Args.GenerateContinousIntegrationConfigs;
                if (generateCIConfigs != null)
                {
                    GenerateCiConfigs(flubuSession, generateCIConfigs, targetsInfo);
                }
                else
                {
                    if (!flubuSession.Args.IsWebApi)
                    {
                        GenerateCiConfigs(flubuSession, _flubuConfiguration.GenerateOnBuild(), targetsInfo);
                    }

                    ExecuteTarget(flubuSession, targetsInfo);
                }
            }
            else
            {
                var targetsInfo = ParseCmdLineArgs(flubuSession.InteractiveArgs.MainCommands, flubuSession.TargetTree);
                ExecuteTarget(flubuSession, targetsInfo);
            }
        }

        private void ExecuteTarget(IFlubuSession flubuSession, (List<string> targetsToRun, bool unknownTarget, List<string> notFoundTargets) targetsInfo)
        {
            flubuSession.Start();

            //// specific target help
            if (targetsInfo.targetsToRun.Count == 2 &&
                targetsInfo.targetsToRun[1].Equals(FlubuTargets.Help, StringComparison.OrdinalIgnoreCase))
            {
                flubuSession.TargetTree.RunTargetHelp(flubuSession, targetsInfo.targetsToRun[0]);
                return;
            }

            if (targetsInfo.targetsToRun.Count == 1 || !flubuSession.Args.ExecuteTargetsInParallel)
            {
                if (targetsInfo.targetsToRun[0].Equals(FlubuTargets.Help, StringComparison.OrdinalIgnoreCase))
                {
                    flubuSession.TargetTree.ScriptArgsHelp = _scriptProperties.GetPropertiesHelp(this);
                }

                BeforeTargetExecution(flubuSession);
                foreach (var targetToRun in targetsInfo.targetsToRun)
                {
                    flubuSession.TargetTree.RunTarget(flubuSession, targetToRun);
                }

                AfterTargetExecution(flubuSession);
            }
            else
            {
                flubuSession.LogInfo("Running targets in parallel.");
                var tasks = new List<Task>();
                BeforeTargetExecution(flubuSession);
                foreach (var targetToRun in targetsInfo.targetsToRun)
                {
                    tasks.Add(flubuSession.TargetTree.RunTargetAsync(flubuSession, targetToRun, true));
                }

                Task.WaitAll(tasks.ToArray());
                AfterTargetExecution(flubuSession);
            }

            if (targetsInfo.unknownTarget)
            {
                var targetNotFoundMsg = $"Target {string.Join(" and ", targetsInfo.notFoundTargets)} not found.";
                if (flubuSession.Args.InteractiveMode)
                {
                    flubuSession.LogInfo(targetNotFoundMsg);
                }
                else
                {
                    throw new TargetNotFoundException(targetNotFoundMsg);
                }
            }

            AssertAllTargetDependenciesWereExecuted(flubuSession);
        }

        protected virtual void BeforeTargetExecution(ITaskContext context)
        {
        }

        protected virtual void AfterTargetExecution(ITaskContext context)
        {
        }

        protected virtual void BeforeBuildExecution(ITaskContext context)
        {
        }

        protected virtual void AfterBuildExecution(IFlubuSession session)
        {
            if (session.Args.GenerateContinousIntegrationConfigs != null &&
                session.Args.GenerateContinousIntegrationConfigs.Count != 0)
            {
                return;
            }

            session.TargetTree.LogBuildSummary(session);
        }

        protected virtual void OnBuildFailed(IFlubuSession session, Exception ex)
        {
        }

        internal void ConfigureTargetsInternal(IFlubuSession flubuSession)
        {
            _flubuSession = flubuSession;
            ConfigureTargets(flubuSession);
        }

        internal void ConfigureBuildPropertiesInternal(IBuildPropertiesContext context)
        {
            ConfigureBuildProperties(context);
        }

        internal void ConfigureDefaultProps(IFlubuSession flubuSession)
        {
            _flubuSession = flubuSession;
            var rootDir = string.IsNullOrEmpty(flubuSession.Args.FlubuFileLocation) ? Directory.GetCurrentDirectory() : Path.GetDirectoryName(flubuSession.Args.FlubuFileLocation);
            flubuSession.Properties.Set(BuildProps.ProductRootDir, rootDir, false);

            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            OSPlatform platform;

            if (!isWindows)
            {
                var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
                platform = isLinux ? OSPlatform.Linux : OSPlatform.OSX;
            }
            else
            {
                platform = OSPlatform.Windows;
            }

            flubuSession.Properties.Set(BuildProps.OSPlatform, platform, false);
            flubuSession.Properties.Set(BuildProps.NodeExecutablePath, IOExtensions.GetNodePath(), false);
            flubuSession.Properties.Set(BuildProps.UserProfileFolder, IOExtensions.GetUserProfileFolder(), false);
            flubuSession.Properties.Set(BuildProps.NpmPath, IOExtensions.GetNpmPath(), false);
            flubuSession.Properties.Set(DotNetBuildProps.BuildDir, RootDirectory.CombineWith("build").ToString());
            flubuSession.Properties.Set(DotNetBuildProps.OutputDir,  RootDirectory.CombineWith("output").ToString(), false);
            flubuSession.Properties.Set(BuildProps.BuildVersion, new BuildVersion(new Version(1, 0, 0, 0)), false);
            flubuSession.Properties.Set(BuildProps.DotNetExecutable, ExecuteDotnetTask.FindDotnetExecutable(), false);
        }

        private void ConfigureDefaultTargets(IFlubuSession flubuSession)
        {
            var defaultTagets = flubuSession.Properties.GetDefaultTargets();

            switch (defaultTagets)
            {
                case DefaultTargets.Dotnet:
                {
                    ConfigureDefaultDotNetTargets(flubuSession);
                    break;
                }
            }
        }

        private void ConfigureDefaultDotNetTargets(IFlubuSession flubuSession)
        {
            var loadSolution = flubuSession.CreateTarget("load.solution")
                .SetDescription("Load & analyze VS solution")
                .AddTask(x => x.LoadSolutionTask())
                .SetAsHidden();

            var cleanOutput = flubuSession.CreateTarget("clean.output")
                .SetDescription("Clean solution outputs")
                .AddTask(x => x.CleanOutputTask())
                .DependsOn(loadSolution);

            var prepareBuildDir = flubuSession.CreateTarget("prepare.build.dir")
                .SetDescription("Prepare the build directory")
                .Do(TargetPrepareBuildDir)
                .SetAsHidden();

            var fetchBuildVersion = flubuSession.CreateTarget("fetch.build.version")
                .SetDescription("Fetch the build version")
                .SetAsHidden();

            var generateCommonAssInfo = flubuSession.CreateTarget("generate.commonassinfo")
                .SetDescription("Generate CommonAssemblyInfo.cs file")
                .DependsOn(fetchBuildVersion)
                .AddTask(x => x.GenerateCommonAssemblyInfoTask());

            flubuSession.CreateTarget("compile")
                .SetDescription("Compile the VS solution")
                .AddTask(x => x.CompileSolutionTask())
                .DependsOn(prepareBuildDir, cleanOutput, generateCommonAssInfo);
        }

        private void GenerateCiConfigs(IFlubuSession flubuSession, List<BuildServerType> generateCIConfigs, (List<string> targetsToRun, bool unknownTarget, List<string> notFoundTargets) targetsInfo)
        {
            YamlConfigurationSerializer serializer = new YamlConfigurationSerializer();
            foreach (var buildServerType in generateCIConfigs)
            {
                switch (buildServerType)
                {
                    case BuildServerType.TravisCI:
                    {
                        TravisConfiguration config = new TravisConfiguration();
                        _flubuConfiguration.TravisOptions.Scripts.Add($"flubu {string.Join(" ", targetsInfo.targetsToRun)}");

                        config.FromOptions(_flubuConfiguration.TravisOptions);

                        var yaml = serializer.Serialize(config);
                        File.WriteAllText(_flubuConfiguration.TravisOptions.ConfigFileName, yaml);
                        flubuSession.LogInfo($"Generated configuration file for travis: {_flubuConfiguration.TravisOptions.ConfigFileName}");
                        break;
                    }

                    case BuildServerType.AzurePipelines:
                    {
                        AzurePipelinesConfiguration config = new AzurePipelinesConfiguration();

                        foreach (var target in flubuSession.TargetTree.GetTargetsInExecutionOrder(flubuSession, false))
                        {
                            _flubuConfiguration.AzurePipelineOptions.AddFlubuTargets(target.TargetName);
                        }

                        for (var i = 0; i < _flubuConfiguration.AzurePipelineOptions.CustomTargets.Count; i++)
                        {
                            _flubuConfiguration.AzurePipelineOptions.CustomTargets[i] =
                                (_flubuConfiguration.AzurePipelineOptions.CustomTargets[i].image,
                                    flubuSession.TargetTree
                                        .GetTargetsInExecutionOrder(_flubuConfiguration.AzurePipelineOptions.CustomTargets[i]
                                            .targets, false)
                                        .Select(x => x.TargetName).ToList());
                        }

                        config.FromOptions(_flubuConfiguration.AzurePipelineOptions);
                        var yaml = serializer.Serialize(config);
                        File.WriteAllText(_flubuConfiguration.AzurePipelineOptions.ConfigFileName, yaml);
                        flubuSession.LogInfo($"Generated configuration file for Azure pipeline: {_flubuConfiguration.AzurePipelineOptions.ConfigFileName}");
                        break;
                    }

                    case BuildServerType.GitHubActions:
                    {
                        GitHubActionsConfiguration config = new GitHubActionsConfiguration();

                        foreach (var target in flubuSession.TargetTree.GetTargetsInExecutionOrder(flubuSession, false))
                        {
                            _flubuConfiguration.GitHubActionsOptions.AddFlubuTargets(target.TargetName);
                        }

                        for (var i = 0; i < _flubuConfiguration.GitHubActionsOptions.CustomTargets.Count; i++)
                        {
                            _flubuConfiguration.GitHubActionsOptions.CustomTargets[i] =
                                (_flubuConfiguration.GitHubActionsOptions.CustomTargets[i].image,
                                    flubuSession.TargetTree
                                        .GetTargetsInExecutionOrder(_flubuConfiguration.GitHubActionsOptions.CustomTargets[i]
                                            .targets, false)
                                        .Select(x => x.TargetName).ToList());
                        }

                        config.FromOptions(_flubuConfiguration.GitHubActionsOptions);

                        var yaml = serializer.Serialize(config);
                        //// Removes ' because yamldotnet adds them when writting [] and this is not valid in github workflows.
                        //// todo do this properly with yamldotnet probably by writing custom type converter.
                        File.WriteAllText(_flubuConfiguration.GitHubActionsOptions.ConfigFileName, yaml.Replace('\'', ' '));
                        flubuSession.LogInfo($"Generated configuration file for GitHub Actions: {_flubuConfiguration.GitHubActionsOptions.ConfigFileName}");
                        break;
                    }

                    case BuildServerType.Jenkins:
                    {
                        JenkinsPipeline configuration = new JenkinsPipeline();

                        foreach (ITargetInternal target in flubuSession.TargetTree.GetTargetsInExecutionOrder(flubuSession))
                        {
                            if (target.TargetHasTasks)
                            {
                                _flubuConfiguration.JenkinsOptions.AddFlubuTargets(target.TargetName);
                            }
                        }

                        if (!_flubuConfiguration.JenkinsOptions.RemoveBuiltInCheckoutStage)
                        {
                            var rt = flubuSession.Tasks()
                                .RunProgramTask("git")
                                .WithArguments("rev-parse", "--is-inside-work-tree")
                                .WithLogLevel(Tasks.LogLevel.None)
                                .DoNotLogTaskExecutionInfo()
                                .DoNotFailOnError()
                                .CaptureOutput()
                                .CaptureErrorOutput();

                            rt.Execute(flubuSession);

                            var errorOutput = rt.GetErrorOutput();

                            if (string.IsNullOrEmpty(errorOutput))
                            {
                                _flubuConfiguration.JenkinsOptions.AddCustomStageBeforeTargets(s =>
                                {
                                    s.Name = "Checkout";
                                    s.AddStep(JenkinsBuiltInSteps.CheckoutStep);
                                });
                            }
                        }

                        configuration.FromOptions(_flubuConfiguration.JenkinsOptions);
                        JenkinsConfigurationSerializer jenkinsConfigurationSerializer = new JenkinsConfigurationSerializer();
                        var jenkinsFile = jenkinsConfigurationSerializer.Serialize(configuration);
                        File.WriteAllText(_flubuConfiguration.JenkinsOptions.ConfigFileName, jenkinsFile);
                        flubuSession.LogInfo($"Generated config for Jenkins: {_flubuConfiguration.JenkinsOptions.ConfigFileName}");
                        break;
                    }

                    case BuildServerType.AppVeyor:
                    {
                        AppVeyorConfiguration config = new AppVeyorConfiguration();
                        _flubuConfiguration.AppVeyorOptions.AddFlubuTargets(targetsInfo.targetsToRun.ToArray());
                        config.FromOptions(_flubuConfiguration.AppVeyorOptions);

                        var yaml = serializer.Serialize(config);
                        File.WriteAllText(_flubuConfiguration.AppVeyorOptions.ConfigFileName, yaml);
                        flubuSession.LogInfo($"Generated configuration file for AppVeyor: {_flubuConfiguration.AppVeyorOptions.ConfigFileName}");
                        break;
                    }
                }
            }
        }

        private void TargetPrepareBuildDir(ITaskContext context)
        {
            var buildDir = context.Properties.Get<string>(DotNetBuildProps.BuildDir);
            var createDirectoryTask = context.Tasks().CreateDirectoryTask(buildDir, true);
            createDirectoryTask.Execute(context);
        }

        private void AssertAllTargetDependenciesWereExecuted(IFlubuSession flubuSession)
        {
            if (flubuSession.Args.TargetsToExecute != null && flubuSession.Args.TargetsToExecute.Count > 1)
            {
                if (flubuSession.Args.TargetsToExecute.Count - 1 != flubuSession.TargetTree.DependenciesExecutedCount)
                {
                    throw new TaskExecutionException("Wrong number of target dependencies were run.", 3);
                }
            }
        }
    }
}