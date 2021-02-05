using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlubuCore.BuildServers.Configurations.Models.GitHubActions;
using NuGet.Packaging.Rules;

namespace FlubuCore.BuildServers.Configurations
{
    public class GitHubActionsOptions
    {
        public GitHubActionsOptions()
        {
            OnPullRequestOptions = new GitHubActionsPullRequestOptions(this);
            OnPushOptions = new GitHubActionsPushOptions(this);
        }

        protected internal string Name { get; set; } = "build";

        protected internal GitHubActionsPullRequestOptions OnPullRequestOptions { get; set; }

        protected internal GitHubActionsPushOptions OnPushOptions { get; set; }

        protected internal string ConfigFileName { get; set; } = "ghActions.generated.yml";

        protected internal bool ShouldGenerateOnEachBuild { get; set; }

        protected internal List<string> VmImages { get; set; } = new List<string>();

        protected internal List<(string image, IGitHubActionStep step)> CustomStepsBeforeTargets { get; set; } = new List<(string image, IGitHubActionStep step)>();

        protected internal List<(string image, IGitHubActionStep step)> CustomStepsAfterTargets { get; set; } = new List<(string image, IGitHubActionStep step)>();

        protected internal List<(string image, List<string> targets)> CustomTargets { get; set; } = new List<(string image, List<string> targets)>();

        protected internal List<string> TargetNames { get; set; } = new List<string>();

        protected internal List<(string image, Dictionary<string, string>)> EnvironmentVariables { get; set; } = new List<(string image, Dictionary<string, string>)>();

        protected internal List<string> PathsOnPush { get; set; } = new List<string>();

        protected internal List<string> PathsToIgnoreOnPush { get; set; } = new List<string>();

        protected internal List<string> TagsToIgnoreOnPush { get; set; } = new List<string>();

        protected internal List<string> TagsOnPush { get; set; } = new List<string>();

        protected internal List<string> BranchesOnPush { get; set; } = new List<string>();

        protected internal List<string> BranchesToIgnoreOnPush { get; set; } = new List<string>();

        protected internal List<string> PathsOnPullRequest { get; set; } = new List<string>();

        protected internal List<string> PathsToIgnoreOnPullRequest { get; set; } = new List<string>();

        protected internal List<string> TagsToIgnoreOnPullRequest { get; set; } = new List<string>();

        protected internal List<string> TagsOnPullRequest { get; set; } = new List<string>();

        protected internal List<string> BranchesOnPullRequest { get; set; } = new List<string>();

        protected internal List<string> BranchesToIgnoreOnPullRequest { get; set; } = new List<string>();

        protected internal string Cron { get; set; }

        protected internal string WorkingDirectory { get; private set; }

        protected internal bool HasAnyOn => HasAnyOnPush || HasAnyOnPullRequests || HasAnySchedules;

        protected internal bool HasAnyOnPush => TagsOnPush.Any() || PathsOnPush.Any() || PathsToIgnoreOnPush.Any() || TagsToIgnoreOnPush.Any() ||
                                                BranchesOnPush.Any() || BranchesToIgnoreOnPush.Any();

        protected internal bool HasAnyOnPullRequests => PathsOnPullRequest.Any() || PathsToIgnoreOnPullRequest.Any() ||
                                                     TagsToIgnoreOnPullRequest.Any() || TagsOnPullRequest.Any() || BranchesOnPullRequest.Any() ||
                                                     BranchesToIgnoreOnPullRequest.Any();

        protected internal bool HasAnySchedules => !string.IsNullOrEmpty(Cron);

        /// <summary>
        /// Set virtual machines images on which you want that your build runs. Default are: Windows-Latest, Linux-Latest, MacOs-Latest.
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public GitHubActionsOptions SetVirtualMachineImage(params GitHubActionsImage[] images)
        {
            images = MapAllLatest(images.ToList()).ToArray();
            foreach (var azurePipelinesImage in images)
            {
               VmImages.Add(MapVmImage(azurePipelinesImage));
            }

            return this;
        }

        /// <summary>
        /// Set virtual machines images on which you want that your build runs. Default are: Windows-Latest, Linux-Latest, MacOs-Latest.
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public GitHubActionsOptions SetVirtualMachineImages(params string[] images)
        {
            VmImages.AddRange(images);
            return this;
        }

        /// <summary>
        /// Set's config file name. Default '.ghActions.generated.yml'.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public GitHubActionsOptions SetConfigFileName(string fileName)
        {
            ConfigFileName = fileName;
            return this;
        }

        /// <summary>
        /// Set's workflow name. Default is 'Build'.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GitHubActionsOptions SetName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        /// When set github actions configuration file is generated on each build.
        /// </summary>
        /// <returns></returns>
        public GitHubActionsOptions GenerateOnEachBuild()
        {
            ShouldGenerateOnEachBuild = true;
            return this;
        }

        /// <summary>
        /// Adds custom step before all flubu targets. Step can be applied for specific image through optional parameter.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public GitHubActionsOptions AddCustomStepBeforeTargets(Action<NameStep> stepOptions, GitHubActionsImage image = GitHubActionsImage.AllLatest)
        {
            NameStep taskItem = new NameStep();
            stepOptions.Invoke(taskItem);
            CustomStepsBeforeTargets.Add((MapVmImage(image), taskItem));
            return this;
        }

        /// <summary>
        /// Adds custom step after all flubu targets. Step can be applied for specific image through optional parameter.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public GitHubActionsOptions AddCustomStepAfterTargets(Action<NameStep> stepOptions, GitHubActionsImage image = GitHubActionsImage.AllLatest)
        {
            NameStep taskItem = new NameStep();
            stepOptions.Invoke(taskItem);
            CustomStepsAfterTargets.Add((MapVmImage(image), taskItem));

            return this;
        }

        /// <summary>
        /// Adds custom step before all flubu targets. Step can be applied for specific image through optional parameter.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public GitHubActionsOptions AddCustomStepBeforeTargets(Action<NameStep> stepOptions, string image)
        {
            NameStep taskItem = new NameStep();
            stepOptions.Invoke(taskItem);
            CustomStepsBeforeTargets.Add((image, taskItem));
            return this;
        }

        /// <summary>
        /// Adds custom step after all flubu targets. Step can be applied for specific image through optional parameter.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public GitHubActionsOptions AddCustomStepAfterTargets(Action<NameStep> stepOptions, string image)
        {
            NameStep taskItem = new NameStep();
            stepOptions.Invoke(taskItem);
            CustomStepsAfterTargets.Add((image, taskItem));
            return this;
        }

        /// <summary>
        /// specified target(s) is used for flubu script generation. Script is applied only to specified image and target specified in command line is ignored for specified image.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public GitHubActionsOptions CustomTargetsForVmImage(GitHubActionsImage image, params string[] targets)
        {
            string img = MapVmImage(image);
            var targetsForImage = CustomTargets.FirstOrDefault(x => x.image == img);
            if (targetsForImage == default)
            {
                CustomTargets.Add((img, targets.ToList()));
            }
            else
            {
               targetsForImage.targets.AddRange(targets);
            }

            return this;
        }

        /// <summary>
        /// specified target(s) is used for flubu script generation. Script is applied only to specified image and target specified in command line is ignored for specified image.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public GitHubActionsOptions CustomTargetsForVmImage(string image, params string[] targets)
        {
            var targetsForImage = CustomTargets.FirstOrDefault(x => x.image == image);
            if (targetsForImage == default)
            {
                CustomTargets.Add((image, targets.ToList()));
            }
            else
            {
                targetsForImage.targets.AddRange(targets);
            }

            return this;
        }

        /// <summary>
        /// Set working directory. All scripts in all jobs are affected.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public GitHubActionsOptions SetWorkingDirectory(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            return this;
        }

        public GitHubActionsOptions SetCronSchedule(string cron)
        {
            Cron = cron;
            return this;
        }

        public GitHubActionsOptions AddEnvironmentVariableToSpecificVmImageJob(string key, string value, params GitHubActionsImage[] images)
        {
            images = MapAllLatest(images.ToList()).ToArray();
            foreach (var image in images)
            {
                var img = MapVmImage(image);
                var envImg = EnvironmentVariables.FirstOrDefault(x => x.image == img);
                if (envImg != default)
                {
                    envImg.Item2[key] = value;
                }
                else
                {
                    EnvironmentVariables.Add((img, new Dictionary<string, string> { { key, value } }));
                }
            }

            return this;
        }

        public GitHubActionsPullRequestOptions OnPullRequest()
        {
            return OnPullRequestOptions;
        }

        public GitHubActionsPushOptions OnPush()
        {
            return OnPushOptions;
        }

        internal GitHubActionsOptions AddFlubuTargets(params string[] targetNames)
        {
            TargetNames.AddRange(targetNames);
            return this;
        }

        private string MapVmImage(GitHubActionsImage image)
        {
            switch (image)
            {
                case GitHubActionsImage.MacOs1015:
                    return "macos-10.15";
                case GitHubActionsImage.MacOsLatest:
                    return "macos-latest";
                case GitHubActionsImage.Ubuntu1604:
                    return "ubuntu-16.04";
                case GitHubActionsImage.Ubuntu1804:
                    return "ubuntu-18-04";
                case GitHubActionsImage.Ubuntu2004:
                    return "ubuntu-18-04";
                case GitHubActionsImage.UbuntuLatest:
                    return "ubuntu-latest";
                case GitHubActionsImage.Windows2019:
                    return "windows-2019";
                case GitHubActionsImage.WindowsLatest:
                    return "windows-latest";
                case GitHubActionsImage.AllLatest:
                    return "all-latest";
                default:
                    throw new NotSupportedException("VmImage mapping not supported.");
            }
        }

        private List<GitHubActionsImage> MapAllLatest(List<GitHubActionsImage> images)
        {
            if (images.Contains(GitHubActionsImage.AllLatest))
            {
                images.Remove(GitHubActionsImage.AllLatest);
                images.Add(GitHubActionsImage.MacOsLatest);
                images.Add(GitHubActionsImage.UbuntuLatest);
                images.Add(GitHubActionsImage.WindowsLatest);
                images = images.Distinct().ToList();
            }

            return images;
        }
    }
}
