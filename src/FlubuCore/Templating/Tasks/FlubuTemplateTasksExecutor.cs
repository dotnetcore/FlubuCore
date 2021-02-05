using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Templating.Models;

namespace FlubuCore.Templating.Tasks
{
    public class FlubuTemplateTasksExecutor : IFlubuTemplateTasksExecutor
    {
        private readonly IFlubuTemplateTaskFactory _flubuTemplateTaskFactory;

        private List<IFlubuTemplateTask> _tasks = new List<IFlubuTemplateTask>();

        public FlubuTemplateTasksExecutor(IFlubuTemplateTaskFactory flubuTemplateTaskFactory)
        {
            _flubuTemplateTaskFactory = flubuTemplateTaskFactory;
        }

        public void SetTasksToExecute(List<string> tasks)
        {
            foreach (var task in tasks)
            {
                _tasks.Add(_flubuTemplateTaskFactory.GetFlubuTemplateTask(task));
            }
        }

        public void AddTaskToExecute(IFlubuTemplateTask task)
        {
            _tasks.Add(task);
        }

        public void BeforeFileProcessing(TemplateModel template, List<string> files)
        {
            ExecuteTasks(task => task.BeforeFileProcessing(template, files));
        }

        public void BeforeFileCopy(string sourcefilePath)
        {
            ExecuteTasks(task => task.BeforeFileCopy(sourcefilePath));
        }

        public void AfterFileCopy(string destinationFilePath)
        {
            ExecuteTasks(task => task.AfterFileCopy(destinationFilePath));
        }

        public void AfterFileProcessing(TemplateModel template)
        {
            ExecuteTasks(task => task.AfterFileProcessing(template));
        }

        private void ExecuteTasks(Action<IFlubuTemplateTask> action)
        {
            foreach (var task in _tasks)
            {
                action.Invoke(task);
            }
        }
    }
}
