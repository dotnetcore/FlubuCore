using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using FlubuCore.Infrastructure;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public class JenkinsPipeline
    {
        public string Agent { get; set; } = "any";

        public JenkinsOptionsDirective Options { get; set; }

        public Dictionary<string, string> Environment { get; set; }

        public List<Stage> Stages { get; set; } = new List<Stage>();

        public List<JenkinsPost> Post { get; set; }

        public string FlubuCommand { get; set; }

        public void FromOptions(JenkinsOptions options)
        {
            switch (options.FlubuToolType)
            {
                case FlubuToolType.GlobalTool:
                    FlubuCommand = "flubu";
                    break;
                case FlubuToolType.LocalTool:
                case FlubuToolType.CliTool:
                    FlubuCommand = "dotnet flubu";
                    break;
            }

            Options = options.Options;

            if (options.Environment != null && options.Environment.Count != 0)
            {
                Environment = options.Environment;
            }

            if (!options.CustomStagesBeforeTargets.IsNullOrEmpty())
            {
                Stages.AddRange(options.CustomStagesBeforeTargets);
            }
            else
            {
                Stages = new List<Stage>();
            }

            foreach (var targetName in options.TargetNames)
            {
                Stage stage = new Stage
                {
                    Name = targetName,
                };

                if (!string.IsNullOrEmpty(options.WorkingDirectory))
                {
                    stage.WorkingDirectory = options.WorkingDirectory;
                }

                var command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bat" : "sh";
                stage.Steps.Add($"{command} '{FlubuCommand} {targetName} --nd'");

                Stages.Add(stage);
            }

            if (!options.CustomStagesAfterTargets.IsNullOrEmpty())
            {
                Stages.AddRange(options.CustomStagesAfterTargets);
            }

            if (!options.JenkinsPosts.IsNullOrEmpty())
            {
                Post = options.JenkinsPosts;
            }
        }
    }
}
