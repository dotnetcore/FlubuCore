using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Process;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public static class TaskFluentInterfaceExtensions
    {
        public static DoTask Do(this ITaskFluentInterface taskF, Action<ITaskContext> taskAction)
        {
            return new DoTask(taskAction);
        }

        public static DoTask2<T> Do<T>(this ITaskFluentInterface taskF, Action<ITaskContextInternal, T> taskAction, T param)
        {
            return new DoTask2<T>(taskAction, param);
        }

        public static DoTask3<T, T2> Do<T, T2>(this ITaskFluentInterface taskF, Action<ITaskContextInternal, T, T2> taskAction, T param, T2 param2)
        {
            return new DoTask3<T, T2>(taskAction, param, param2);
        }

        public static DoTask4<T, T2, T3> Do<T, T2, T3>(this ITaskFluentInterface taskF, Action<ITaskContextInternal, T, T2, T3> taskAction, T param, T2 param2, T3 param3)
        {
            return new DoTask4<T, T2, T3>(taskAction, param, param2, param3);
        }

        public static DoTask5<T, T2, T3, T4> Do<T, T2, T3, T4>(this ITaskFluentInterface taskF, Action<ITaskContextInternal, T, T2, T3, T4> taskAction, T param, T2 param2, T3 param3, T4 param4)
        {
            return new DoTask5<T, T2, T3, T4>(taskAction, param, param2, param3, param4);
        }

        public static DoTask6<T, T2, T3, T4, T5> Do<T, T2, T3, T4, T5>(this ITaskFluentInterface taskF, Action<ITaskContextInternal, T, T2, T3, T4, T5> taskAction, T param, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return new DoTask6<T, T2, T3, T4, T5>(taskAction, param, param2, param3, param4, param5);
        }

        public static DoTaskAsync DoAsync(this ITaskFluentInterface taskF, Func<ITaskContext, Task> taskAction)
        {
            return new DoTaskAsync(taskAction);
        }

        public static DoTaskAsync2<T> DoAsync<T>(this ITaskFluentInterface taskF, Func<ITaskContextInternal, T, Task> taskAction, T param)
        {
            return new DoTaskAsync2<T>(taskAction, param);
        }

        public static DoTaskAsync3<T, T2> DoAsync<T, T2>(this ITaskFluentInterface taskF, Func<ITaskContextInternal, T, T2, Task> taskAction, T param, T2 param2)
        {
            return new DoTaskAsync3<T, T2>(taskAction, param, param2);
        }

        public static DoTaskAsync4<T, T2, T3> DoAsync<T, T2, T3>(this ITaskFluentInterface taskF, Func<ITaskContextInternal, T, T2, T3, Task> taskAction, T param, T2 param2, T3 param3)
        {
            return new DoTaskAsync4<T, T2, T3>(taskAction, param, param2, param3);
        }

        public static DoTaskAsync5<T, T2, T3, T4> DoAsync<T, T2, T3, T4>(this ITaskFluentInterface taskF, Func<ITaskContextInternal, T, T2, T3, T4, Task> taskAction, T param, T2 param2, T3 param3, T4 param4)
        {
            return new DoTaskAsync5<T, T2, T3, T4>(taskAction, param, param2, param3, param4);
        }

        public static DoTaskAsync6<T, T2, T3, T4, T5> DoAsync<T, T2, T3, T4, T5>(this ITaskFluentInterface taskF, Func<ITaskContextInternal, T, T2, T3, T4, T5, Task> taskAction, T param, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return new DoTaskAsync6<T, T2, T3, T4, T5>(taskAction, param, param2, param3, param4, param5);
        }

        /// <summary>
        /// Run's multiple programs.
        /// </summary>
        /// <param name="programs"></param>
        /// <returns></returns>
        public static List<IRunProgramTask> RunMultiProgram(this ITaskFluentInterface taskF, params string[] programs)
        {
            var fluent = (TaskFluentInterface)taskF;
            List<IRunProgramTask> runProgramTasks = new List<IRunProgramTask>();
            foreach (string program in programs)
            {
               runProgramTasks.Add(CreateRunProgram(fluent.Context, program, null));
            }

            return runProgramTasks;
        }

        /// <summary>
        /// Run specified program.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="workingFolder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IRunProgramTask RunProgram(this ITaskFluentInterface taskF, string program, string workingFolder, params string[] args)
        {
            var fluent = (TaskFluentInterface)taskF;
            return CreateRunProgram(fluent.Context, program, workingFolder, args);
        }

        /// <summary>
        /// Run specified program.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="workingFolder"></param>
        /// <returns></returns>
        public static IRunProgramTask RunProgram(this ITaskFluentInterface taskF, string program, string workingFolder, Action<IRunProgramTask> action = null)
        {
            var fluent = (TaskFluentInterface)taskF;
            var task = CreateRunProgram(fluent.Context, program, workingFolder, null);
            action?.Invoke(task);
            return task;
        }

        private static IRunProgramTask CreateRunProgram(TaskContext context, string program, string workingFolder, params string[] args)
        {
            var task = context.Tasks().RunProgramTask(program);

            return task
                .WorkingFolder(workingFolder)
                .WithArguments(args);
        }
    }
}
