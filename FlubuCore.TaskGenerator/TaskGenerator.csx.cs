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
     public partial class {task.TaskName}Task<TTask> : ExternalProcessTaskBase<TTask> where TTask : class, ITask
     {{
        public {task.TaskName}Task({GetConstructorParameters(task)})
        {{
            {GetConstructorArguments(task)}
        }}

        protected override string Description {{ get; set; }}
     }}
}}
");
        }

        public string GetConstructorArguments(Task task)
        {
            string arguments = string.Empty;
            foreach (var argument in task.Constructor.Arguments)
            {
                arguments = $"{arguments}{GetArgument(argument)}";
            }

            return arguments;
        }

        private static string GetArgument(Argument argument)
        {
            if (argument.HasArgumentValue)
            {
                return $"WithArgumentsRequiredValue(\"{argument.ArgumentKey}\", {argument.Parameter.ParameterName});{Environment.NewLine}";
            }
            else
            {
                return $"WithArguments(\"{argument.ArgumentKey}\");{Environment.NewLine}";
            }
        }

        public string GetConstructorParameters(Task task)
        {
            string parameters = string.Empty;
            foreach (var argument in task.Constructor.Arguments)
            {
                if (argument.Parameter != null)
                {
                    parameters = $"{parameters} {GetParameter(argument.Parameter)} ";
                }
            }

            parameters = parameters.Trim();
            return parameters;
        }

        public string GetMethods(Task task)
        {
            string methods = string.Empty;
            foreach (var method in task.Methods)
            {
            }

            return methods;
        }


        private string GetParameter(Parameter parameter)
        {
            return $"{parameter.ParameterType} {parameter.ParameterName}";
        }
    }
}
