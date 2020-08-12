using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlubuCore.BuildServers.Configurations.Models.AzurePipelines.Job;
using FlubuCore.Infrastructure;

namespace FlubuCore.BuildServers.Configurations.Models.AzurePipelines
{
    public class AzurePipelinesConfiguration
    {
        public List<string> Trigger { get; set; }

        public List<string> Variables { get; set; }

        public List<JobItem> Jobs { get; set; } = new List<JobItem>();

        public void FromOptions(AzurePipelineOptions options)
        {
            if (options.VmImages.IsNullOrEmpty())
            {
                options.SetVirtualMachineImage(AzurePipelinesImage.WindowsLatest, AzurePipelinesImage.UbuntuLatest, AzurePipelinesImage.MacOsLatest);
            }

            if (!options.Triggers.IsNullOrEmpty())
            {
                Trigger = options.Triggers;
            }

            if (!options.Variables.IsNullOrEmpty())
            {
                Variables = options.Variables;
            }

            foreach (var vmImage in options.VmImages)
            {
                var job = new JobItem();
                job.Job = vmImage.Replace("-", "_");

                job.Pool = new Pool()
                {
                    VmImage = vmImage
                };

                job.AddStep(new TaskItem
                {
                    DisplayName = "Install .net core sdk",
                    Task = "DotNetCoreInstaller@1",
                    Inputs = new TaskInputs
                    {
                        Version = "3.1.302"
                    }
                });

                job.AddStep(new ScriptItem
                {
                    DisplayName = "Install flubu",
                    Script = "dotnet tool install --global FlubuCore.Tool --version 5.1.8"
                });

                foreach (var customStepsBeforeTarget in options.CustomStepsBeforeTargets)
                {
                    if (customStepsBeforeTarget.image == "all" || customStepsBeforeTarget.image == vmImage)
                    {
                        if (string.IsNullOrEmpty(customStepsBeforeTarget.step.WorkingDirectory))
                        {
                            customStepsBeforeTarget.step.WorkingDirectory = options.WorkingDirectory;
                        }

                        if (customStepsBeforeTarget.step is ScriptItem scriptItem)
                        {
                            job.AddStep(scriptItem.Clone());
                        }
                        else if (customStepsBeforeTarget.step is TaskItem taskItem)
                        {
                            job.AddStep(taskItem.Clone());
                        }
                    }
                }

                var customTarget = options.CustomTargets.FirstOrDefault(x => x.image == vmImage);
                if (customTarget != default)
                {
                    foreach (var targetName in customTarget.targets)
                    {
                        job.AddStep(new ScriptItem()
                        {
                            DisplayName = targetName,
                            WorkingDirectory = options.WorkingDirectory,
                            Script = $"flubu {targetName} --nd"
                        });
                    }
                }
                else
                {
                    foreach (var targetName in options.TargetNames)
                    {
                        job.AddStep(new ScriptItem()
                        {
                            DisplayName = targetName,
                            WorkingDirectory = options.WorkingDirectory,
                            Script = $"flubu {targetName} --nd"
                        });
                    }
                }

                foreach (var customStepsAfterTarget in options.CustomStepsAfterTargets)
                {
                    if (customStepsAfterTarget.image == "all" || customStepsAfterTarget.image == vmImage)
                    {
                        if (string.IsNullOrEmpty(customStepsAfterTarget.step.WorkingDirectory))
                        {
                            customStepsAfterTarget.step.WorkingDirectory = options.WorkingDirectory;
                        }

                        if (customStepsAfterTarget.step is ScriptItem scriptItem)
                        {
                            job.AddStep(scriptItem.Clone());
                        }
                        else if (customStepsAfterTarget.step is TaskItem taskItem)
                        {
                            job.AddStep(taskItem.Clone());
                        }
                    }
                }

                Jobs.Add(job);
            }
        }
    }
}
