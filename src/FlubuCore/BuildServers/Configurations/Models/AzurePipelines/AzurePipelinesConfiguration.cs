using System;
using System.Collections.Generic;
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
                options.AddVirtualMachineImage(AzurePipelinesImage.WindowsLatest, AzurePipelinesImage.UbuntuLatest, AzurePipelinesImage.MacOsLatest);
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
                job.Pool = new Pool()
                {
                    VmImage = vmImage
                };

                foreach (var customStepsBeforeTarget in options.CustomStepsBeforeTargets)
                {
                    if (customStepsBeforeTarget.image == "all" || customStepsBeforeTarget.image == vmImage)
                    {
                        if (string.IsNullOrEmpty(customStepsBeforeTarget.step.WorkingDirectory))
                        {
                            customStepsBeforeTarget.step.WorkingDirectory = options.WorkingDirectory;
                        }

                        var scriptItem = customStepsBeforeTarget.step as ScriptItem;

                        if (scriptItem != null)
                        {
                            job.AddStep(new ScriptItem
                            {
                                Script = scriptItem.Script,
                                DisplayName = scriptItem.DisplayName,
                                WorkingDirectory = scriptItem.WorkingDirectory
                            });
                        }
                    }
                }

                foreach (var targetName in options.TargetNames)
                {
                    job.AddStep(new ScriptItem()
                    {
                        DisplayName = targetName,
                        WorkingDirectory = options.WorkingDirectory,
                        Script = $"flubu {targetName}"
                    });
                }

                foreach (var customStepsAfterTarget in options.CustomStepsAfterTargets)
                {
                    if (customStepsAfterTarget.image == "all" || customStepsAfterTarget.image == vmImage)
                    {
                        if (string.IsNullOrEmpty(customStepsAfterTarget.step.WorkingDirectory))
                        {
                            customStepsAfterTarget.step.WorkingDirectory = options.WorkingDirectory;
                        }

                        var scriptItem = customStepsAfterTarget.step as ScriptItem;

                        if (scriptItem != null)
                        {
                            job.AddStep(new ScriptItem
                            {
                                Script = scriptItem.Script,
                                DisplayName = scriptItem.DisplayName,
                                WorkingDirectory = scriptItem.WorkingDirectory
                            });
                        }
                    }
                }

                Jobs.Add(job);
            }
        }
    }
}
