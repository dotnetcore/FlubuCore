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

        public AppVeyorOptions SetVirtualMachineImage(params AppVeyorImage[] images)
        {
            foreach (var image in images)
            {
                Images.Add(MapVmImage(image));
            }

            return this;
        }

        public AppVeyorOptions SetCloneDepth(int cloneDepth)
        {
            CloneDepth = cloneDepth;
            return this;
        }

        public AppVeyorOptions AddSkipCommits(params string[] fileOrPattern)
        {
            SkipCommitsFiles.AddRange(fileOrPattern);
            return this;
        }

        public AppVeyorOptions SetWorkingDirectory(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            return this;
        }

        public AppVeyorOptions AddServices(params string[] services)
        {
            Services.AddRange(services);
            return this;
        }

        public AppVeyorOptions AddArtifacts(params string[] artifacts)
        {
            Artifacts.AddRange(artifacts);
            return this;
        }

        public AppVeyorOptions AddCustomScriptBeforeTarget(string script, string image)
        {
            CustomScriptsBeforeTarget.Add((image, script));
            return this;
        }

        public AppVeyorOptions AddCustomScriptBeforeTarget(string script, AppVeyorImage image = AppVeyorImage.All)
        {
            CustomScriptsBeforeTarget.Add((MapVmImage(image), script));
            return this;
        }

        public AppVeyorOptions AddCustomScriptAfterTarget(string script, AppVeyorImage image = AppVeyorImage.All)
        {
            CustomScriptsAfterTarget.Add((MapVmImage(image), script));
            return this;
        }

        public AppVeyorOptions AddCustomScriptAfterTarget(string script, string image)
        {
            CustomScriptsAfterTarget.Add((image, script));
            return this;
        }

        public AppVeyorOptions AddBeforeScript(string script, AppVeyorImage image = AppVeyorImage.All)
        {
            BeforeBuilds.Add((MapVmImage(image), script));
            return this;
        }

        public AppVeyorOptions AddBeforeScript(string script, string image)
        {
            BeforeBuilds.Add((image, script));
            return this;
        }

        public AppVeyorOptions AddCustomTarget(string targetName, AppVeyorImage image)
        {
            Targets.Add((MapVmImage(image), targetName));
            return this;
        }

        public AppVeyorOptions AddCustomTarget(string targetName, string image)
        {
            Targets.Add((image, targetName));
            return this;
        }

        public AppVeyorOptions SetConfigFileName(string filename)
        {
            ConfigFileName = filename;
            return this;
        }

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
