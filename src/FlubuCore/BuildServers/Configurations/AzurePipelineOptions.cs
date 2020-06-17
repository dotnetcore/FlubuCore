using System;
using System.Collections.Generic;
using System.Linq;
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

        protected internal string ConfigFileName { get; set; } = "azure-pipelines.generated.yml";

        protected internal bool ShouldGenerateOnEachBuild { get; set; }

        protected internal List<(string image, List<string> targets)> CustomTargets { get; set; } = new List<(string image, List<string> targets)>();

        /// <summary>
        /// Set virtual machines images on which you want that your build runs. Default are: Windows-Latest, Linux-Latest, MacOs-Latest.
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public AzurePipelineOptions SetVirtualMachineImage(params AzurePipelinesImage[] images)
        {
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
        public AzurePipelineOptions SetVirtualMachineImages(params string[] images)
        {
            VmImages.AddRange(images);
            return this;
        }

        /// <summary>
        /// Continuous integration (CI) triggers cause a pipeline to run whenever you push an update to the specified branches or you push specified tags.
        /// </summary>
        /// <param name="triggers"></param>
        /// <returns></returns>
        public AzurePipelineOptions AddTriggers(params string[] triggers)
        {
            Triggers.AddRange(triggers);
            return this;
        }

        /// <summary>
        /// Add Variables.
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        public AzurePipelineOptions AddVariables(params string[] variables)
        {
            Variables.AddRange(variables);
            return this;
        }

        /// <summary>
        /// Set working directory. All scripts in all jobs are affected.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public AzurePipelineOptions SetWorkingDirectory(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            return this;
        }

        /// <summary>
        /// Set generated configuration file name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public AzurePipelineOptions SetConfigFileName(string fileName)
        {
            ConfigFileName = fileName;
            return this;
        }

        /// <summary>
        /// When applied Azure pipelines configuration file is generated on each build.
        /// </summary>
        /// <returns></returns>
        public AzurePipelineOptions GenerateOnEachBuild()
        {
            ShouldGenerateOnEachBuild = true;
            return this;
        }

        /// <summary>
        /// Adds custom script step before all flubu targets. Script can be applied for specific image through optional parameter.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public AzurePipelineOptions AddCustomScriptStepBeforeTargets(Action<ScriptItem> scriptOptions, AzurePipelinesImage image = AzurePipelinesImage.All)
        {
            ScriptItem scriptItem = new ScriptItem();
            scriptOptions.Invoke(scriptItem);
            CustomStepsBeforeTargets.Add((MapVmImage(image), scriptItem));
            return this;
        }

        /// <summary>
        /// Adds custom script step after all flubu targets. Script can be applied for specific image through optional parameter.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public AzurePipelineOptions AddCustomScriptStepAfterTargets(Action<ScriptItem> scriptOptions, AzurePipelinesImage image = AzurePipelinesImage.All)
        {
            ScriptItem scriptItem = new ScriptItem();
            scriptOptions.Invoke(scriptItem);
            CustomStepsAfterTargets.Add((MapVmImage(image), scriptItem));

            return this;
        }

        /// <summary>
        /// Adds custom script step before all flubu targets. Script can be applied for specific image through optional parameter.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public AzurePipelineOptions AddCustomScriptStepBeforeTargets(Action<ScriptItem> scriptOptions, string image)
        {
            ScriptItem scriptItem = new ScriptItem();
            scriptOptions.Invoke(scriptItem);
            CustomStepsBeforeTargets.Add((image, scriptItem));
            return this;
        }

        /// <summary>
        /// Adds custom script step after all flubu targets. Script can be applied for specific image through optional parameter.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public AzurePipelineOptions AddCustomScriptStepAfterTargets(Action<ScriptItem> scriptOptions, string image)
        {
            ScriptItem scriptItem = new ScriptItem();
            scriptOptions.Invoke(scriptItem);
            CustomStepsAfterTargets.Add((image, scriptItem));

            return this;
        }

        /// <summary>
        /// Adds custom task step before all flubu targets. Task can be applied for specific image through optional parameter.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public AzurePipelineOptions AddCustomTaskStepBeforeTargets(Action<TaskItem> scriptOptions, AzurePipelinesImage image = AzurePipelinesImage.All)
        {
            TaskItem taskItem = new TaskItem();
            scriptOptions.Invoke(taskItem);
            CustomStepsBeforeTargets.Add((MapVmImage(image), taskItem));
            return this;
        }

        /// <summary>
        /// Adds custom task step after all flubu targets. Task can be applied for specific image through optional parameter.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public AzurePipelineOptions AddCustomTaskStepAfterTargets(Action<TaskItem> scriptOptions, AzurePipelinesImage image = AzurePipelinesImage.All)
        {
            TaskItem taskItem = new TaskItem();
            scriptOptions.Invoke(taskItem);
            CustomStepsAfterTargets.Add((MapVmImage(image), taskItem));

            return this;
        }

        /// <summary>
        /// Adds custom task step before all flubu targets. Task can be applied for specific image through optional parameter.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public AzurePipelineOptions AddCustomTaskStepBeforeTargets(Action<TaskItem> scriptOptions, string image)
        {
            TaskItem taskItem = new TaskItem();
            scriptOptions.Invoke(taskItem);
            CustomStepsBeforeTargets.Add((image, taskItem));
            return this;
        }

        /// <summary>
        /// Adds custom task step after all flubu targets. Task can be applied for specific image through optional parameter.
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <param name="image">script is applied to specified image</param>
        /// <returns></returns>
        public AzurePipelineOptions AddCustomTaskStepAfterTargets(Action<TaskItem> scriptOptions, string image)
        {
            TaskItem taskItem = new TaskItem();
            scriptOptions.Invoke(taskItem);
            CustomStepsAfterTargets.Add((image, taskItem));

            return this;
        }

        /// <summary>
        /// specified target(s) is used for flubu script generation. Script is applied only to specified image and target specified in command line is ignored for specified image.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public AzurePipelineOptions CustomTargetsForVmImage(AzurePipelinesImage image, params string[] targets)
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
        public AzurePipelineOptions CustomTargetsForVmImage(string image, params string[] targets)
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
