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

        public void GenerateTasks(List<Task> tasks)
        {
            foreach (var task in tasks)
            {
                GenerateTask(task);
            }
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

namespace {task.Namespace}
{{
     public partial class {task.TaskName} : ExternalProcessTaskBase<{task.TaskName}>
     {{
        public {task.TaskName}({GetConstructorParameters(task)})
        {{
            {GetConstructorArguments(task)}
        }}

        protected override string Description {{ get; set; }}
        {GetMethods(task)}
     }}
}}
");
        }

        public string GetConstructorArguments(Task task)
        {
            string arguments = string.Empty;
            foreach (var argument in task.Constructor.Arguments)
            {
                arguments = $"{arguments}{GetArgument(argument)}{Environment.NewLine}";
            }

            arguments = arguments.Remove(arguments.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            return arguments;
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
                methods = $@"{methods}
        public {task.TaskName} {method.MethodName}({GetParameter(method.Argument?.Parameter)})
        {{
            {GetArgument(method.Argument)}
            return this;
        }}";
            }

            return methods;
        }

        private static string GetArgument(Argument argument)
        {
            if (argument == null)
            {
                return string.Empty;
            }

            if (argument.HasArgumentValue)
            {
                return $"WithArgumentsValueRequired(\"{argument.ArgumentKey}\", {argument.Parameter.ParameterName});";
            }
            else
            {
                return $"WithArguments(\"{argument.ArgumentKey}\");";
            }
        }


        private string GetParameter(Parameter parameter)
        {
            if (parameter == null)
            {
                return string.Empty;
            }

            return $"{parameter.ParameterType} {parameter.ParameterName}";
        }
    }
}
