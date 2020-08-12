using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlubuCore.BuildServers.Configurations.Models.AzurePipelines;
using FlubuCore.BuildServers.Configurations.Models.AzurePipelines.Job;
using FlubuCore.Infrastructure;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace FlubuCore.BuildServers.Configurations.Models.GitHubActions
{
    public class GitHubActionsConfiguration
    {
        public string Name { get; set; }

        public On On { get; set; }

        [YamlMember(Alias = "on")]
        public string On2 { get; set; }

        public Dictionary<string, GitHubActionJob> Jobs { get; set; } = new Dictionary<string, GitHubActionJob>();

        public void FromOptions(GitHubActionsOptions options)
        {
            Name = options.Name;

            if (options.VmImages.IsNullOrEmpty())
            {
                options.SetVirtualMachineImage(GitHubActionsImage.WindowsLatest, GitHubActionsImage.UbuntuLatest, GitHubActionsImage.MacOsLatest);
            }

            if (options.HasAnyOn)
            {
                On = new On();
            }
            else
            {
                On2 = "[push, pull_request]";
            }

            if (options.HasAnyOnPullRequests)
            {
                On.PullRequest = new PullRequest();

                if (!options.PathsOnPullRequest.IsNullOrEmpty())
                {
                    On.PullRequest.PathsInclude = options.PathsOnPullRequest;
                }

                if (!options.PathsToIgnoreOnPullRequest.IsNullOrEmpty())
                {
                    On.PullRequest.PathsIgnore = options.PathsToIgnoreOnPullRequest;
                }

                if (!options.TagsOnPullRequest.IsNullOrEmpty())
                {
                    On.PullRequest.Tags = options.TagsOnPullRequest;
                }

                if (!options.TagsToIgnoreOnPullRequest.IsNullOrEmpty())
                {
                    On.PullRequest.TagsIgnore = options.TagsToIgnoreOnPullRequest;
                }

                if (!options.BranchesOnPullRequest.IsNullOrEmpty())
                {
                    On.PullRequest.Branches = options.BranchesOnPullRequest;
                }

                if (!options.BranchesToIgnoreOnPullRequest.IsNullOrEmpty())
                {
                    On.PullRequest.BranchesIgnore = options.BranchesToIgnoreOnPullRequest;
                }
            }

            if (options.HasAnyOnPush)
            {
                On.Push = new Push();
                if (!options.PathsOnPush.IsNullOrEmpty())
                {
                    On.Push.PathsInclude = options.PathsOnPush;
                }

                if (!options.PathsToIgnoreOnPush.IsNullOrEmpty())
                {
                    On.Push.PathsIgnore = options.PathsToIgnoreOnPush;
                }

                if (!options.BranchesToIgnoreOnPush.IsNullOrEmpty())
                {
                    On.Push.BranchesIgnore = options.BranchesToIgnoreOnPush;
                }

                if (!options.BranchesOnPush.IsNullOrEmpty())
                {
                    On.Push.Branches = options.BranchesOnPush;
                }

                if (!options.TagsOnPush.IsNullOrEmpty())
                {
                    On.Push.Tags = options.TagsOnPush;
                }

                if (!options.TagsToIgnoreOnPush.IsNullOrEmpty())
                {
                    On.Push.TagsIgnore = options.TagsToIgnoreOnPush;
                }
            }

            if (options.HasAnySchedules)
            {
                On.Schedule = new Schedule();
                if (!string.IsNullOrEmpty(options.Cron))
                {
                    On.Schedule.Cron = options.Cron;
                }
            }

            foreach (var vmImage in options.VmImages)
            {
                GitHubActionJob job = new GitHubActionJob();
                job.RunsOn = vmImage;
                job.Name = vmImage;

                var envVariables = options.EnvironmentVariables.FirstOrDefault(x => x.image == vmImage);

                if (envVariables != default)
                {
                    job.Env = envVariables.Item2;
                }

                job.AddStep(new NameStep
                {
                    Name = "Checkout",
                    Uses = "actions/checkout@v2"
                });

                job.AddStep(new NameStep
                {
                    Name = "Install Flubu",
                    Run = "dotnet tool install --global FlubuCore.Tool --version 5.1.8"
                });

                int stepCount = 1;
                foreach (var customStepsBeforeTarget in options.CustomStepsBeforeTargets)
                {
                    if (customStepsBeforeTarget.image == "all-latest" || customStepsBeforeTarget.image == vmImage)
                    {
                        if (customStepsBeforeTarget.step is NameStep nameStep)
                        {
                            if (string.IsNullOrEmpty(nameStep.WorkingDirectory))
                            {
                                nameStep.WorkingDirectory = options.WorkingDirectory;
                            }

                            if (string.IsNullOrEmpty(nameStep.Name))
                            {
                                nameStep.Name = $"Step {stepCount}";
                            }

                            job.AddStep(nameStep.Clone());
                        }
                    }
                }

                var customTarget = options.CustomTargets.FirstOrDefault(x => x.image == vmImage);
                if (customTarget != default)
                {
                    foreach (var targetName in customTarget.targets)
                    {
                        job.AddStep(new NameStep()
                        {
                            Name = targetName,
                            WorkingDirectory = options.WorkingDirectory,
                            Run = $"flubu {targetName} --nd"
                        });
                    }
                }
                else
                {
                    foreach (var targetName in options.TargetNames)
                    {
                        job.AddStep(new NameStep()
                        {
                            Name = targetName,
                            WorkingDirectory = options.WorkingDirectory,
                            Run = $"flubu {targetName} --nd"
                        });
                    }
                }

                foreach (var customStepsAfterTarget in options.CustomStepsAfterTargets)
                {
                    if (customStepsAfterTarget.image == "all-latest" || customStepsAfterTarget.image == vmImage)
                    {
                        if (customStepsAfterTarget.step is NameStep nameStep)
                        {
                            if (string.IsNullOrEmpty(nameStep.WorkingDirectory))
                            {
                                nameStep.WorkingDirectory = options.WorkingDirectory;
                            }

                            if (string.IsNullOrEmpty(nameStep.Name))
                            {
                                nameStep.Name = $"Step {stepCount}";
                            }

                            job.AddStep(nameStep.Clone());
                        }
                    }
                }

                stepCount++;
                Jobs.Add(vmImage, job);
            }
        }
    }
}
