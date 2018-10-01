using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.TaskGenerator.Models;
using Microsoft.CodeAnalysis;
using Scripty.Core;

namespace FlubuCore.TaskGenerator
{
     public class TaskGenerator
    {
        private readonly ScriptContext _context;

        public TaskGenerator(ScriptContext context)
        {
            _context = context;
        }

        public void GenerateTask(Task task)
        {
            _context.Output[task.FileName]
                .WriteLine($@"
using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Process;

namespace FlubuCore.{task.ProjectName}.Tasks
{{
     public class {task.TaskName}Task<TTask> : ExternalProcessTaskBase<TTask> where TTask : class, ITask
     {{
        public {task.TaskName}Task()
        {{
            
        }}
     }}
}}
");
        }

        public string GetConstructorParameters(Task task)
        {
            string parameters = string.Empty;
            foreach (var parameter in task.ConstructorParameters)
            {
                parameters = $"{parameters} {parameter.ParameterType} {parameter.ParameterName} ";
            }

            return parameters;
        }
    }
}
