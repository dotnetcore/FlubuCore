using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.BuildServers.Configurations.Models.AzurePipelines.Job;

namespace FlubuCore.BuildServers.Configurations
{
    public class AzurePipelineOptions
    {
        protected internal List<string> Triggers { get; set; } = new List<string>();

        protected internal List<string> Variables { get; set; } = new List<string>();

        protected internal List<string> VmImages { get; set; } = new List<string>();

        protected internal List<string> TargetNames { get; set; } = new List<string>();

        protected internal List<(string image, IStep step)> CustomStepsBeforeTargets { get; set; } = new List<(string image, IStep step)>();

        protected internal List<(string image, IStep step)> CustomStepsAfterTargets { get; set; } = new List<(string image, IStep step)>();

        protected internal string WorkingDirectory { get; set; }

        public AzurePipelineOptions AddVirtualMachineImage(params AzurePipelinesImage[] images)
        {
            foreach (var azurePipelinesImage in images)
            {
               VmImages.Add(MapVmImage(azurePipelinesImage));
            }

            return this;
        }

        public AzurePipelineOptions AddVirtualMachineImage(params string[] images)
        {
            VmImages.AddRange(images);
            return this;
        }

        public AzurePipelineOptions AddTriggers(params string[] triggers)
        {
            Triggers.AddRange(triggers);
            return this;
        }

        public AzurePipelineOptions AddVariables(params string[] variables)
        {
            Variables.AddRange(variables);
            return this;
        }

        public AzurePipelineOptions SetWorkingDirectory(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            return this;
        }

        public AzurePipelineOptions SetConfigFileName(string fileName)
        {
            return this;
        }

        public AzurePipelineOptions GenerateOnEachBuild()
        {
            return this;
        }

        public AzurePipelineOptions AddCustomScriptStepBeforeTargets(Action<ScriptItem> scriptOptions, AzurePipelinesImage image = AzurePipelinesImage.All)
        {
            ScriptItem scriptItem = new ScriptItem();
            scriptOptions.Invoke(scriptItem);
            CustomStepsBeforeTargets.Add((MapVmImage(image), scriptItem));
            return this;
        }

        public AzurePipelineOptions AddCustomScriptStepAfterTargets(Action<ScriptItem> scriptOptions, AzurePipelinesImage image = AzurePipelinesImage.All)
        {
            ScriptItem scriptItem = new ScriptItem();
            scriptOptions.Invoke(scriptItem);
            CustomStepsAfterTargets.Add((MapVmImage(image), scriptItem));

            return this;
        }

        public AzurePipelineOptions AddCustomScriptStepBeforeTargets(Action<ScriptItem> scriptOptions, string image)
        {
            ScriptItem scriptItem = new ScriptItem();
            scriptOptions.Invoke(scriptItem);
            CustomStepsBeforeTargets.Add((image, scriptItem));
            return this;
        }

        public AzurePipelineOptions AddCustomScriptStepAfterTargets(Action<ScriptItem> scriptOptions, string image)
        {
            ScriptItem scriptItem = new ScriptItem();
            scriptOptions.Invoke(scriptItem);
            CustomStepsAfterTargets.Add((image, scriptItem));

            return this;
        }

        public AzurePipelineOptions AddCustomTaskStepBeforeTargets(Action<TaskItem> scriptOptions, AzurePipelinesImage image = AzurePipelinesImage.All)
        {
            TaskItem taskItem = new TaskItem();
            scriptOptions.Invoke(taskItem);
            CustomStepsBeforeTargets.Add((MapVmImage(image), taskItem));
            return this;
        }

        public AzurePipelineOptions AddCustomTaskStepAfterTargets(Action<TaskItem> scriptOptions, AzurePipelinesImage image = AzurePipelinesImage.All)
        {
            TaskItem taskItem = new TaskItem();
            scriptOptions.Invoke(taskItem);
            CustomStepsAfterTargets.Add((MapVmImage(image), taskItem));

            return this;
        }

        public AzurePipelineOptions AddCustomTaskStepBeforeTargets(Action<TaskItem> scriptOptions, string image)
        {
            TaskItem taskItem = new TaskItem();
            scriptOptions.Invoke(taskItem);
            CustomStepsBeforeTargets.Add((image, taskItem));
            return this;
        }

        public AzurePipelineOptions AddCustomTaskStepAfterTargets(Action<TaskItem> scriptOptions, string image)
        {
            TaskItem taskItem = new TaskItem();
            scriptOptions.Invoke(taskItem);
            CustomStepsAfterTargets.Add((image, taskItem));

            return this;
        }

        public AzurePipelineOptions CustomTargetsForVmImage(AzurePipelinesImage image, params string[] targets)
        {
            return this;
        }

        internal AzurePipelineOptions AddFlubuTargets(params string[] targetNames)
        {
            TargetNames.AddRange(targetNames);
            return this;
        }

        private static string MapVmImage(AzurePipelinesImage azurePipelinesImage)
        {
            switch (azurePipelinesImage)
            {
                case AzurePipelinesImage.MacOsLatest:
                    return "macOs-latest";

                case AzurePipelinesImage.UbuntuLatest:
                    return "ubuntu-latest";

                case AzurePipelinesImage.WindowsLatest:
                    return "windows-latest";
                case AzurePipelinesImage.All:
                    return "all";

                default:
                    throw new NotSupportedException("VmImage mapping not supported.");
            }
        }
    }
}
