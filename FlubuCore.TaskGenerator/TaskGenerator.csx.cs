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

        public virtual void GenerateTask(Task task)
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
            ExecutablePath = ""{task.ExecutablePath}"";
            {WriteConstructorArguments(task)}
        }}

        protected override string Description {{ get; set; }}
        {WriteMethods(task)}
     }}
}}");
        }

        protected internal virtual string WriteConstructorArguments(Task task)
        {
            string arguments = string.Empty;
            foreach (var argument in task.Constructor.Arguments)
            {
                arguments = $"{arguments}{WriteArgument(argument)}{Environment.NewLine}";
            }

            arguments = arguments.Remove(arguments.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            return arguments;
        }

        protected internal virtual string WriteConstructorParameters(Task task)
        {
            string parameters = string.Empty;
            foreach (var argument in task.Constructor.Arguments)
            {
                if (argument.Parameter != null)
                {
                    if (!string.IsNullOrEmpty(parameters))
                    {
                        parameters = $"{parameters}, ";
                    }

                    parameters = $"{parameters} {WriteParameter(argument.Parameter)}";
                }
            }

            parameters = parameters.Trim();
            return parameters;
        }

        protected internal virtual string WriteMethods(Task task)
        {
            string methods = string.Empty;

            if (task.Methods == null || task.Methods.Count == 0)
            {
                return methods;
            }
           
            foreach (var method in task.Methods)
            {
                methods = $@"{methods}{WriteSummary(method.MethodSummary)}
        public {task.TaskName} {method.MethodName}({WriteParameter(method.Argument?.Parameter)})
        {{
            {WriteArgument(method.Argument)}
            return this;
        }}" + Environment.NewLine;
            }

            methods = methods.Remove(methods.Length - Environment.NewLine.Length, Environment.NewLine.Length);

            return methods;
        }

        protected internal virtual string WriteArgument(Argument argument)
        {
            if (argument == null)
            {
                return string.Empty;
            }

            if (argument.HasArgumentValue)
            {
                string parameterName = ParameterName(argument.Parameter.ParameterName);

                return $"WithArgumentsValueRequired(\"{argument.ArgumentKey}\", {parameterName});";
            }
            else
            {
                return $"WithArguments(\"{argument.ArgumentKey}\");";
            }
        }

        protected internal virtual string WriteParameter(Parameter parameter)
        {
            if (parameter == null)
            {
                return string.Empty;
            }

            string parameterName = ParameterName(parameter.ParameterName);

            return $"{parameter.ParameterType} {parameterName}";

        }

        protected internal virtual string WriteSummary(string summary)
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

        protected internal virtual string ParameterName(string parameterName)
        {
            if (parameterName.Equals("namespace") || parameterName.Equals("params") || parameterName.Equals("operator") ||
                parameterName.Equals("new") || parameterName.Equals("override"))
            {
                parameterName = $"@{parameterName}";
            }

            return parameterName;
        }
    }
}
