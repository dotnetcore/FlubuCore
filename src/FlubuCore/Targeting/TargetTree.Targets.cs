using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.Infrastructure.Terminal;
using FlubuCore.Scripting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FlubuCore.Targeting
{
    public partial class TargetTree
    {
        /// <summary>
        ///     The target for displaying help in the command line.
        /// </summary>
        /// <param name="context">The task context.</param>
        public virtual void LogTargetsWithHelp(ITaskContextInternal context)
        {
            if (context != null && !string.IsNullOrEmpty(context.Args.FlubuHelpText))
            {
                context.LogInfo(context.Args.FlubuHelpText);
            }

            LogTargetsHelp(context);
        }

        public virtual void LogTargetsHelp(ITaskContextInternal context)
        {
            context.DecreaseDepth();
            context.LogInfo("Targets:");

            // first sort the targets
            var sortedTargets = new SortedList<string, ITargetInternal>();

            foreach (var target in _targets.Values)
            {
                sortedTargets.Add(target.TargetName, target);
            }

            // now display them in sorted order
            foreach (ITargetInternal target in sortedTargets.Values)
            {
                if (target.IsHidden == false)
                {
                    string help = $"  {target.TargetName.Capitalize()}";

                    if (target.Dependencies != null && target.Dependencies.Count != 0)
                    {
                        help = $"{help} ({string.Join(", ", target.Dependencies.GetKeys())})";
                    }

                    help = $"{help} : {target.Description}";

                    if (DefaultTargets.Contains(target))
                    {
                        context.LogInfo(help, Color.DarkOrange);
                    }
                    else
                    {
                        context.LogInfo(help);
                    }
                }
            }

            if (ScriptArgsHelp?.Count > 0)
            {
                context.LogInfo(" ");
                context.LogInfo("Global build script arguments:");
                foreach (var argHelp in ScriptArgsHelp)
                {
                    context.LogInfo($"  {argHelp}");
                }
            }

            context.IncreaseDepth();
        }

        public virtual void LogTasksHelp(ITaskContextInternal context)
        {
            context.LogInfo("Tasks:");

            IEnumerable<ITask> tasks = _provider.GetServices<ITask>();

            foreach (ITask task in tasks)
            {
                context.LogInfo($"  {task.GetType().FullName}");
            }
        }
    }
}
