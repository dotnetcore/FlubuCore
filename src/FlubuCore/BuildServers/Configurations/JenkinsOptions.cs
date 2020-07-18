using System;
using System.Collections.Generic;
using FlubuCore.BuildServers.Configurations.Models.GitHubActions;
using FlubuCore.BuildServers.Configurations.Models.Jenkins;

namespace FlubuCore.BuildServers.Configurations
{
    public class JenkinsOptions
    {
        private JenkinsPostOptions _postOptions;

        protected internal JenkinsOptionsDirective Options { get; set; }

        protected internal string WorkingDirectory { get; set; }

        protected internal bool ShouldGenerateOnEachBuild { get; set; }

        protected internal string ConfigFileName { get; set; } = "Jenkinsfile.generated";

        protected internal List<string> TargetNames { get; set; } = new List<string>();

        protected internal List<Stage> CustomStagesBeforeTargets { get; set; } = new List<Stage>();

        protected internal List<Stage> CustomStagesAfterTargets { get; set; } = new List<Stage>();

        protected internal List<JenkinsPost> JenkinsPosts { get; set; } = new List<JenkinsPost>();

        public JenkinsOptions ConfigureOptions(Action<JenkinsOptionsDirective> optionsAction)
        {
            JenkinsOptionsDirective options = new JenkinsOptionsDirective();

            optionsAction.Invoke(options);
            Options = options;
            return this;
        }

        /// <summary>
        /// Adds custom stage before all flubu targets.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public JenkinsOptions AddCustomStageBeforeTargets(Action<Stage> stageOptions)
        {
            Stage taskItem = new Stage();
            stageOptions.Invoke(taskItem);
            CustomStagesBeforeTargets.Add(taskItem);
            return this;
        }

        /// <summary>
        /// Adds custom stage after all flubu targets.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public JenkinsOptions AddCustomStageAfterTargets(Action<Stage> stageOptions)
        {
            Stage taskItem = new Stage();
            stageOptions.Invoke(taskItem);
            CustomStagesAfterTargets.Add(taskItem);
            return this;
        }

        /// <summary>
        /// Set working directory. All steps are affected except the one with specific working directory.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public JenkinsOptions SetWorkingDirectory(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            return this;
        }

        /// <summary>
        /// Set's config file name. Default '.ghActions.generated.yml'.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public JenkinsOptions SetConfigFileName(string fileName)
        {
            ConfigFileName = fileName;
            return this;
        }

        public JenkinsOptions ConfigurePostSteps(Action<JenkinsPostOptions> postOptionsAction)
        {
            if (_postOptions == null)
            {
                _postOptions = new JenkinsPostOptions(this);
            }

            postOptionsAction.Invoke(_postOptions);
            return this;
        }

        /// <summary>
        /// When set github actions configuration file is generated on each build.
        /// </summary>
        /// <returns></returns>
        public JenkinsOptions GenerateOnEachBuild()
        {
            ShouldGenerateOnEachBuild = true;
            return this;
        }

        internal JenkinsOptions AddFlubuTargets(params string[] targetNames)
        {
            TargetNames.AddRange(targetNames);
            return this;
        }
    }
}
