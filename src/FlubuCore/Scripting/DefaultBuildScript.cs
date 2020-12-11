﻿using System;
using System.Collections.Generic;
using System.IO;
#if !NETSTANDARD1_6
using System.Drawing;
#endif
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FlubuCore.Context;
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

        /// <summary>
        /// Get's product root directory.
        /// </summary>
        protected FullPath RootDirectory => new FullPath(_flubuSession.Properties.GetProductRootDir());

        public int Run(IFlubuSession flubuSession)
        {
            _flubuSession = flubuSession;
            _scriptProperties = flubuSession.ScriptFactory.CreateScriptProperties();
            _targetCreator = flubuSession.ScriptFactory.CreateTargetCreator();

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
#if !NETSTANDARD1_6
                    flubuSession.LogError($"ERROR: {e.Message}", Color.Red);
#else
                    flubuSession.LogError($"error: {e.Message}");
#endif
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
#if !NETSTANDARD1_6
                    flubuSession.LogError($"ERROR: {e}", Color.Red);
#else
                    flubuSession.LogError($"error: {e}");
#endif
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

        public virtual void Configure(ILoggerFactory loggerFactory)
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

                ExecuteTarget(flubuSession, targetsInfo);
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