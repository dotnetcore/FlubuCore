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
        {WriteSummary(task.Constructor?.Summary)}
        public {task.TaskName}({WriteConstructorParameters(task)})
        {{
            {WriteConstructorArguments(task)}
        }}

        protected override string Description {{ get; set; }}
        {WriteMethods(task)}
     }}
}}
");
        }

        public string WriteConstructorArguments(Task task)
        {
            string arguments = string.Empty;
            foreach (var argument in task.Constructor.Arguments)
            {
                arguments = $"{arguments}{WriteArgument(argument)}{Environment.NewLine}";
            }

            arguments = arguments.Remove(arguments.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            return arguments;
        }

        public string WriteConstructorParameters(Task task)
        {
            string parameters = string.Empty;
            foreach (var argument in task.Constructor.Arguments)
            {
                if (argument.Parameter != null)
                {
                    parameters = $"{parameters} {WriteParameter(argument.Parameter)} ";
                }
            }

            parameters = parameters.Trim();
            return parameters;
        }

        public string WriteMethods(Task task)
        {
            string methods = string.Empty;
            foreach (var method in task.Methods)
            {
                methods = $@"{methods}{WriteSummary(method.MethodSummary)}
        public {task.TaskName} {method.MethodName}({WriteParameter(method.Argument?.Parameter)})
        {{
            {WriteArgument(method.Argument)}
            return this;
        }}";
            }

            return methods;
        }

        private static string WriteArgument(Argument argument)
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


        private string WriteParameter(Parameter parameter)
        {
            if (parameter == null)
            {
                return string.Empty;
            }

            return $"{parameter.ParameterType} {parameter.ParameterName}";
        }

        private string WriteSummary(string summary)
        {
            if (string.IsNullOrEmpty(summary))
            {
                return null;
            }

            return $@"
        /// <summary>
        /// {summary}
        /// </summary>";
        }

    }
}
