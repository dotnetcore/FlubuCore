using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.BuildServers.Configurations.Models.Jenkins;

namespace FlubuCore.BuildServers.Configurations
{
    public class JenkinsBuiltInStageOptions
    {
        private JenkinsOptions _options;

        public JenkinsBuiltInStageOptions(JenkinsOptions options)
        {
            _options = options;
        }

        public JenkinsBuiltInStageOptions RemoveDefaultBuiltInCheckoutStage()
        {
            _options.RemoveBuiltInCheckoutStage = true;
            return this;
        }

        public JenkinsBuiltInStageOptions AddArchiveArtifactsStep(string fileGlobbingExpresion)
        {
            _options.AddCustomStageAfterTargets(s =>
            {
                s.Name = "Archive artifacts";
                s.AddStep($"archiveArtifacts '{fileGlobbingExpresion}'");
            });

            return this;
        }
    }
}
