using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using flubu.Tasks;
using flubu.Targeting;

namespace Flubu.Builds
{
    /// <summary>
    /// Built in build targets.
    /// </summary>
    public static class BuildTargets
    {
        public static void FillBuildTargets(TargetTree targetTree)
        {

            var loadSolution = new Target(targetTree, "load.solution")
               .SetDescription("Load & analyze VS solution")
               .Do(TargetLoadSolution)
               .SetAsHidden();

            targetTree.AddTarget(loadSolution);

            var cleanOutput = new Target(targetTree, "clean.output")
                .SetDescription("Clean solution outputs")
                .DependsOn(loadSolution)
                .Do(TargetCleanOutput);

            targetTree.AddTarget(cleanOutput);

            targetTree.AddTarget("before.compile")
                .SetDescription("Steps before compiling the VS solution")
                .DependsOn("prepare.build.dir", "load.solution", "clean.output", "generate.commonassinfo")
                .SetAsHidden();

            targetTree.AddTarget("compile")
                .SetDescription("Compile the VS solution")
                .DependsOn("before.compile")
                .Do(TargetCompile);

            targetTree.AddTarget("fetch.build.version")
                .SetDescription("Fetch the build version")
                .SetAsHidden();

            targetTree.AddTarget("fxcop")
                .SetDescription("Run FxCop")
                .Do(TargetFxCop);

            targetTree.AddTarget("generate.commonassinfo")
                .SetDescription("Generate CommonAssemblyInfo.cs file")
                .DependsOn("fetch.build.version")
                .Do(TargetGenerateCommonAssemblyInfo);

           

            targetTree.AddTarget("prepare.build.dir")
                .SetDescription("Prepare the build directory")
                .Do(TargetPrepareBuildDir)
                .SetAsHidden ();
        }

        public static void FillDefaultProperties(ITaskContext context)
        {
            //todo implement
            throw new NotImplementedException();
            //context.Properties.Set(BuildProps.BuildConfiguration, "Release");
            //context.Properties.Set(BuildProps.BuildDir, "Builds");
            //context.Properties.Set(BuildProps.BuildLogsDir, @"Builds\BuildLogs");
            //context.Properties.Set(BuildProps.FxCopDir, "Microsoft FxCop 1.36");
            //context.Properties.Set(BuildProps.LibDir, "lib");
            //context.Properties.Set(BuildProps.PackagesDir, "packages");
            //context.Properties.Set(BuildProps.ProductRootDir, ".");
        }

        public static Version FetchBuildVersionFromFile (ITaskContext context)
        {
            //todo implement
            throw new NotImplementedException();
            //string productRootDir = context.Properties.Get (BuildProps.ProductRootDir, ".");
            //string projectVersionFileName = context.Properties.Get<string>(BuildProps.ProjectVersionFileName, null);
            //string productId = context.Properties.Get<string>(BuildProps.ProductId);
            
            //IFetchBuildVersionTask task = new FetchBuildVersionFromFileTask (productRootDir, productId)
            //                                  {
            //                                      ProjectVersionFileName = projectVersionFileName
            //                                  };
            //task.Execute (context);
            //return task.BuildVersion;
        }

        [SuppressMessage ("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static int FetchBuildNumberFromFile (ITaskContext context)
        {
            //todo implement
            throw new NotImplementedException();
            //string productRootDir = context.Properties.Get (BuildProps.ProductRootDir, ".");
            //string productId = context.Properties.Get<string> (BuildProps.ProductId);
            //string projectBuildNumberFileName = Path.Combine (productRootDir, productId + ".BuildNumber.txt");

            //if (false == File.Exists (projectBuildNumberFileName))
            //    return 1;

            //using (Stream stream = File.Open (projectBuildNumberFileName, FileMode.Open))
            //{
            //    using (StreamReader reader = new StreamReader (stream))
            //    {
            //        string buildNumberAsString = reader.ReadLine ();
            //        try
            //        {
            //            return int.Parse(buildNumberAsString, CultureInfo.InvariantCulture);
            //        }
            //        catch
            //        {
            //            return 1;
            //        }
            //    }
            //}
        }

        public static void IncrementBuildNumberInFile (ITaskContext context)
        {
            //todo implement
            throw new NotImplementedException();
            //string productRootDir = context.Properties.Get (BuildProps.ProductRootDir, ".");
            //string productId = context.Properties.Get<string> (BuildProps.ProductId);
            //string projectBuildNumberFileName = Path.Combine (productRootDir, productId + ".BuildNumber.txt");

            //int nextBuildNumber = context.Properties.Get<Version>(BuildProps.BuildVersion).Build + 1;
            //File.WriteAllText(projectBuildNumberFileName, nextBuildNumber.ToString(CultureInfo.InvariantCulture));

            //context.WriteInfo("Incrementing the next build number to {0}", nextBuildNumber);
        }

        public static void OnBuildFinished (ITaskSession session)
        {
            session.ResetDepth();

            LogTargetDurations(session);

            //todo implement
            throw new NotImplementedException();

            //session.WriteInfo(String.Empty);

            //if (session.HasFailed)
            //    session.WriteError("BUILD FAILED");
            //else
            //    session.WriteInfo("BUILD SUCCESSFUL");

            //TimeSpan buildDuration = session.BuildStopwatch.Elapsed;
            //session.WriteInfo("Build finish time: {0:g}", DateTime.Now);
            //session.WriteInfo(
            //    "Build duration: {0:D2}:{1:D2}:{2:D2} ({3:d} seconds)", 
            //    buildDuration.Hours, 
            //    buildDuration.Minutes, 
            //    buildDuration.Seconds, 
            //    (int)buildDuration.TotalSeconds);

            //bool speechDisabled = session.Properties.Get(BuildProps.SpeechDisabled, false);
            //if (session.IsInteractive && !speechDisabled)
            //{
            //    using (SpeechSynthesizer speech = new SpeechSynthesizer())
            //    {
            //        PromptBuilder builder = new PromptBuilder(new CultureInfo("en-US"));
            //        builder.StartStyle(new PromptStyle(PromptRate.Slow));
            //        builder.StartStyle(new PromptStyle(PromptVolume.Loud));
            //        builder.StartSentence(new CultureInfo("en-US"));
            //        builder.AppendText("Build " + (session.HasFailed ? "failed." : "successful!"));
            //        builder.EndSentence();
            //        builder.EndStyle();
            //        builder.EndStyle();
            //        speech.Speak(builder);
            //    }
            //}

            ////Beeper.Beep(session.HasFailed ? MessageBeepType.Error : MessageBeepType.Ok);
        }

        public static void LogTargetDurations(ITaskSession session)
        {
            if (session.TargetTree == null)
                return;

            //todo implement
            throw new NotImplementedException();

        //    session.WriteInfo(String.Empty);

            //    SortedList<string, ITarget> sortedTargets = new SortedList<string, ITarget>();

            //    foreach (ITarget target in session.TargetTree.EnumerateExecutedTargets())
            //        sortedTargets.Add(target.TargetName, target);

            //    foreach (ITarget target in sortedTargets.Values)
            //    {
            //        if (target.TaskStopwatch.ElapsedTicks > 0)
            //        {
            //            session.WriteInfo(
            //                "Target {0} took {1} s", 
            //                target.TargetName, 
            //                (int)target.TaskStopwatch.Elapsed.TotalSeconds);
            //        }
            //    }
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public static void TargetCleanOutput(ITaskContext context)
        {
            //todo implement
            throw new NotImplementedException();

            //string buildConfiguration = context.Properties.Get<string>(BuildProps.BuildConfiguration);
            //string productRootDir = context.Properties.Get(BuildProps.ProductRootDir, ".");

            //VSSolution solution = context.Properties.Get<VSSolution>(BuildProps.Solution);

            //solution.ForEachProject(
            //    delegate(VSProjectInfo projectInfo)
            //        {
            //            if (projectInfo is VSProjectWithFileInfo)
            //            {
            //                VSProjectWithFileInfo info = (VSProjectWithFileInfo)projectInfo;

            //                LocalPath projectOutputPath = info.GetProjectOutputPath(buildConfiguration);

            //                if (projectOutputPath == null)
            //                    return;

            //                FullPath projectFullOutputPath = info.ProjectDirectoryPath.CombineWith(projectOutputPath);
            //                DeleteDirectoryTask.Execute(context, projectFullOutputPath.ToString(), false);

            //                string projectObjPath = String.Format(
            //                    CultureInfo.InvariantCulture, 
            //                    @"{0}\obj\{1}", 
            //                    projectInfo.ProjectName, 
            //                    buildConfiguration);
            //                projectObjPath = Path.Combine(productRootDir, projectObjPath);
            //                DeleteDirectoryTask.Execute(context, projectObjPath, false);
            //            }
            //        });
        }

        public static void TargetCompile(ITaskContext context)
        {
            //todo implement
            throw new NotImplementedException();

            //VSSolution solution = context.Properties.Get<VSSolution>(BuildProps.Solution);
            //string buildConfiguration = context.Properties.Get<string>(BuildProps.BuildConfiguration);
            //string toolsVersion = context.Properties.Get<string>(BuildProps.MSBuildToolsVersion, null);
            //bool useSolutionDirAsMsBuildWorkingDir = context.Properties.Get(BuildProps.UseSolutionDirAsMSBuildWorkingDir, false);

            //CompileSolutionTask task = new CompileSolutionTask(
            //    solution.SolutionFileName.ToString(), 
            //    buildConfiguration);

            //if (toolsVersion != null)
            //{
            //    Version toolsVersionObj;

            //    if (!Version.TryParse(toolsVersion, out toolsVersionObj))
            //    {
            //        context.Fail (
            //            "Property '{0}' value '{1}' is invalid, it has to be a proper version number", 
            //            BuildProps.MSBuildToolsVersion, 
            //            toolsVersion);
            //        return;
            //    }

            //    task.ToolsVersion = toolsVersionObj;
            //}

            //task.UseSolutionDirAsWorkingDir = useSolutionDirAsMsBuildWorkingDir;
            //task.MaxCpuCount = context.Properties.Get(BuildProps.CompileMaxCpuCount, 3);
            //task.Execute(context);
        }

        public static void TargetFxCop(ITaskContext context)
        {

            //todo implement
            throw new NotImplementedException();

            //FullPath rootDir = new FullPath(context.Properties[BuildProps.ProductRootDir]);

            //FullPath fxcopDir = rootDir
            //    .CombineWith(context.Properties[BuildProps.LibDir])
            //    .CombineWith(context.Properties[BuildProps.FxCopDir]);

            //FullPath buildLogsPath = new FullPath(context.Properties[BuildProps.ProductRootDir])
            //    .CombineWith(context.Properties[BuildProps.BuildLogsDir]);

            //RunFxCopTask task = new RunFxCopTask(
            //    fxcopDir.AddFileName("FxCopCmd.exe").ToString(), 
            //    fxcopDir.AddFileName("FxCop.exe").ToString(), 
            //    rootDir.AddFileName("{0}.FxCop", context.Properties[BuildProps.ProductId]).ToString(), 
            //    buildLogsPath.AddFileName("{0}.FxCopReport.xml", context.Properties[BuildProps.ProductId]).ToString());
            //task.Execute(context);
        }

        public static void TargetGenerateCommonAssemblyInfo(ITaskContext context)
        {
            //todo implement
            throw new NotImplementedException();

            //string buildConfiguration = context.Properties.Get<string>(BuildProps.BuildConfiguration);
            //Version buildVersion = context.Properties.Get<Version>(BuildProps.BuildVersion);
            //string companyCopyright = context.Properties.Get(BuildProps.CompanyCopyright, String.Empty);
            //string companyName = context.Properties.Get(BuildProps.CompanyName, String.Empty);
            //string companyTrademark = context.Properties.Get(BuildProps.CompanyTrademark, String.Empty);
            //string productId = context.Properties.Get<string>(BuildProps.ProductId);
            //string productName = context.Properties.Get(BuildProps.ProductName, productId);
            //string productRootDir = context.Properties.Get(BuildProps.ProductRootDir, ".");
            //bool generateAssemblyVersion = context.Properties.Get(BuildProps.AutoAssemblyVersion, true);

            //GenerateCommonAssemblyInfoTask task = new GenerateCommonAssemblyInfoTask(productRootDir, buildVersion);
            //task.BuildConfiguration = buildConfiguration;
            //task.CompanyCopyright = companyCopyright;
            //task.CompanyName = companyName;
            //task.CompanyTrademark = companyTrademark;
            //task.GenerateConfigurationAttribute = true;
            //task.ProductName = productName;

            //if (context.Properties.Has (BuildProps.InformationalVersion))
            //    task.InformationalVersion = context.Properties.Get<string>(BuildProps.InformationalVersion);

            //task.ProductVersionFieldCount = context.Properties.Get(BuildProps.ProductVersionFieldCount, 2);
            //task.GenerateAssemblyVersion = generateAssemblyVersion;
            //task.Execute(context);
        }

        public static void TargetLoadSolution(ITaskContext context)
        {
            //todo implement
            throw new NotImplementedException();

            //string solutionFileName = context.Properties.Get<string>(BuildProps.SolutionFileName);
            //VSSolution solution = VSSolution.Load(solutionFileName);
            //context.Properties.Set(BuildProps.Solution, solution);

            //solution.ForEachProject(delegate(VSProjectInfo projectInfo)
            //{
            //    if (projectInfo.ProjectTypeGuid != VSProjectType.CSharpProjectType.ProjectTypeGuid)
            //        return;

            //    //projectExtendedInfos.Add(
            //    //    projectInfo.ProjectName,
            //    //    new VSProjectExtendedInfo(projectInfo));
            //});

            //// also load project files
            //solution.LoadProjects();
        }

        public static void TargetPrepareBuildDir(ITaskContext context)
        {
            //todo implement
            throw new NotImplementedException();
            //string buildDir = context.Properties.Get<string>(BuildProps.BuildDir);
            //CreateDirectoryTask createDirectoryTask = new CreateDirectoryTask(buildDir, true);
            //createDirectoryTask.Execute(context);
        }

        public static void TargetRunTestsNUnit(
            ITaskContext context, 
            string projectName)
        {
            //todo implement
            throw new NotImplementedException();
            //VSSolution solution = context.Properties.Get<VSSolution>(BuildProps.Solution);
            //string buildConfiguration = context.Properties.Get<string>(BuildProps.BuildConfiguration);

            //VSProjectWithFileInfo project =
            //    (VSProjectWithFileInfo)solution.FindProjectByName(projectName);
            //FileFullPath projectTarget = project.ProjectDirectoryPath.CombineWith(project.GetProjectOutputPath(buildConfiguration))
            //    .AddFileName("{0}.dll", project.ProjectName);

            //IRunProgramTask task = new RunProgramTask(
            //    context.Properties[BuildProps.NUnitConsolePath])
            //    .AddArgument(projectTarget.ToString())
            //    .AddArgument("/labels")
            //    .AddArgument("/trace=Verbose")
            //    .AddArgument("/nodots")
            //    .AddArgument("/noshadow");
            //task.Execute(context);
        }
    }
}