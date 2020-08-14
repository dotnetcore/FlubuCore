using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations
{
    public class AppVeyorOptions
    {
        protected internal string ConfigFileName { get; set; } = "appveyor.generated.yml";

        protected internal bool ShouldGenerateOnEachBuild { get; set; }

        protected internal List<string> Images { get; set; } = new List<string>();

        protected internal int? CloneDepth { get; set; }

        protected internal string WorkingDirectory { get; set; }

        protected internal List<string> SkipCommitsFiles { get; set; } = new List<string>();

        protected internal List<string> Services { get; set; } = new List<string>();

        protected internal List<string> Artifacts { get; set; } = new List<string>();

        protected internal List<(string image, string script)> CustomScriptsBeforeTarget { get; set; } = new List<(string image, string script)>();

        protected internal List<(string image, string script)> CustomScriptsAfterTarget { get; set; } = new List<(string image, string script)>();

        protected internal List<(string image, string target)> Targets { get; set;  } = new List<(string image, string target)>();

        protected internal List<(string image, string beforeScript)> BeforeBuilds { get; set; } = new List<(string image, string beforeScript)>();

        protected internal List<string> TargetNames { get; set; } = new List<string>();

        /// <summary>
        /// Set virtual machines images on which you want that your build runs. Default are: Visual Studio 2019, Ubuntu1804
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public AppVeyorOptions SetVirtualMachineImage(params AppVeyorImage[] images)
        {
            foreach (var image in images)
            {
                Images.Add(MapVmImage(image));
            }

            return this;
        }

        /// <summary>
        /// Set's 'clone_depth:' entry. clone entire repository history if not defined
        /// </summary>
        /// <param name="cloneDepth"></param>
        /// <returns></returns>
        public AppVeyorOptions SetCloneDepth(int cloneDepth)
        {
            CloneDepth = cloneDepth;
            return this;
        }

        /// <summary>
        /// Skipping commits affecting specific files.
        /// </summary>
        /// <param name="fileOrPattern"></param>
        /// <returns></returns>
        public AppVeyorOptions AddSkipCommits(params string[] fileOrPattern)
        {
            SkipCommitsFiles.AddRange(fileOrPattern);
            return this;
        }

        /// <summary>
        /// Set's working directory where script(s) are executed.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public AppVeyorOptions SetWorkingDirectory(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            return this;
        }

        /// <summary>
        /// enable service required for build/tests
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public AppVeyorOptions AddServices(params string[] services)
        {
            Services.AddRange(services);
            return this;
        }

        /// <summary>
        /// artifacts configuration.
        /// </summary>
        /// <param name="artifacts"></param>
        /// <returns></returns>
        public AppVeyorOptions AddArtifacts(params string[] artifacts)
        {
            Artifacts.AddRange(artifacts);
            return this;
        }

        /// <summary>
        /// Adds 'build_script:' entries before Flubu target script entry.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public AppVeyorOptions AddCustomScriptBeforeTarget(string script, string image)
        {
            CustomScriptsBeforeTarget.Add((image, script));
            return this;
        }

        /// <summary>
        /// Adds 'build_script:' entries before Flubu target script entry.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public AppVeyorOptions AddCustomScriptBeforeTarget(string script, AppVeyorImage image = AppVeyorImage.All)
        {
            CustomScriptsBeforeTarget.Add((MapVmImage(image), script));
            return this;
        }

        /// <summary>
        /// Adds 'build_script:' entries after Flubu target script entry.
        /// </summary>
        /// <param name="scriptAfterTarget"></param>
        /// <returns></returns>
        public AppVeyorOptions AddCustomScriptAfterTarget(string script, AppVeyorImage image = AppVeyorImage.All)
        {
            CustomScriptsAfterTarget.Add((MapVmImage(image), script));
            return this;
        }

        /// <summary>
        /// Adds 'build_script:' entries after Flubu target script entry.
        /// </summary>
        /// <param name="scriptAfterTarget"></param>
        /// <returns></returns>
        public AppVeyorOptions AddCustomScriptAfterTarget(string script, string image)
        {
            CustomScriptsAfterTarget.Add((image, script));
            return this;
        }

        /// <summary>
        /// Adds 'before_script:' entries.
        /// </summary>
        /// <param name="scriptAfterTarget"></param>
        /// <returns></returns>
        public AppVeyorOptions AddBeforeScript(string script, AppVeyorImage image = AppVeyorImage.All)
        {
            BeforeBuilds.Add((MapVmImage(image), script));
            return this;
        }

        /// <summary>
        /// Adds 'build_script:' entries after Flubu target script entry.
        /// </summary>
        /// <param name="scriptAfterTarget"></param>
        /// <returns></returns>
        public AppVeyorOptions AddBeforeScript(string script, string image)
        {
            BeforeBuilds.Add((image, script));
            return this;
        }

        /// <summary>
        /// specified target(s) is used for flubu script generation. Script is applied only to specified image and target specified in command line is ignored for specified image.
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public AppVeyorOptions AddCustomTarget(string targetName, AppVeyorImage image)
        {
            Targets.Add((MapVmImage(image), targetName));
            return this;
        }

        /// <summary>
        /// specified target(s) is used for flubu script generation. Script is applied only to specified image and target specified in command line is ignored for specified image.
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public AppVeyorOptions AddCustomTarget(string targetName, string image)
        {
            Targets.Add((image, targetName));
            return this;
        }

        /// <summary>
        /// Set generated configuration file name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public AppVeyorOptions SetConfigFileName(string filename)
        {
            ConfigFileName = filename;
            return this;
        }

        /// <summary>
        /// When applied Azure pipelines configuration file is generated on each build.
        /// </summary>
        /// <returns></returns>
        public AppVeyorOptions GenerateOnEachBuild()
        {
            ShouldGenerateOnEachBuild = true;
            return this;
        }

        internal AppVeyorOptions AddFlubuTargets(params string[] targetNames)
        {
            TargetNames.AddRange(targetNames);
            return this;
        }

        private static string MapVmImage(AppVeyorImage image)
        {
            switch (image)
            {
                case AppVeyorImage.Ubuntu1804:
                    return "Ubuntu1804";
                case AppVeyorImage.VisualStudio2019:
                    return "Visual Studio 2019";
                case AppVeyorImage.All:
                    return "All";
                default:
                    throw new NotSupportedException($"Image '{image}' not supported.");
            }
        }
    }
}
