using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Infrastructure;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public class JenkinsPipeline
    {
        public string Agent { get; set; } = "any";

        public JenkinsOptionsDirective Options { get; set; }

        public List<Stage> Stages { get; set; } = new List<Stage>();

        public List<JenkinsPost> Post { get; set; }

        public void FromOptions(JenkinsOptions options)
        {
            Options = options.Options;

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

                stage.Steps.Add($"flubu {targetName} --nd");

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
