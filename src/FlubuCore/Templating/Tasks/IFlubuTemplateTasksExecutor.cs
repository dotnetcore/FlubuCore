using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Templating.Models;

namespace FlubuCore.Templating.Tasks
{
    public interface IFlubuTemplateTasksExecutor
    {
        void SetTasksToExecute(List<string> tasks);

        void AddTaskToExecute(IFlubuTemplateTask task);

        void BeforeFileProcessing(TemplateModel template, List<string> files);

        void BeforeFileCopy(string sourcefilePath);

        void AfterFileCopy(string destinationFilePath);

        void AfterFileProcessing(TemplateModel template);
    }
}
