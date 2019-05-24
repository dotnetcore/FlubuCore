using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Flubu.VisualStudio.Base.TaskRunner;
using Microsoft.VisualStudio.TaskRunnerExplorer;

namespace Flubu.VisualStudio.TaskRunner
{
    [TaskRunnerExport("buildscript.cs")]
    public class TaskRunnerProvider : ITaskRunner
    {
        private readonly HashSet<string> _dynamicNames = new HashSet<string>(new TrimmingStringComparer('\u200B'));
        private readonly ImageSource _icon;

        public TaskRunnerProvider()
        {
            _icon = new BitmapImage(new Uri(@"pack://application:,,,/CommandTaskRunner;component/Resources/project.png"));
        }

        public async Task<ITaskRunnerConfig> ParseConfig(ITaskRunnerCommandContext context, string configPath)
        {
            return await Task.Run(() =>
            {
                var hierarchy = LoadHierarchy(configPath);

                if (!hierarchy.Children.Any() && !hierarchy.Children.First().Children.Any())
                    return null;

                return new TaskRunnerConfig(this, context, hierarchy, _icon);
            });
        }

        public List<ITaskRunnerOption> Options { get; } = null;

        private ITaskRunnerNode LoadHierarchy(string configPath)
        {
            ITaskRunnerNode root = new TaskRunnerNode("Commands");
            var rootDir = Path.GetDirectoryName(configPath);
            var commands = new List<CommandTask> {new CommandTask {Name = "build"}};

            var tasks = new TaskRunnerNode("Commands") {Description = "A list of command to execute"};
            root.Children.Add(tasks);

            foreach (var command in commands.OrderBy(k => k.Name))
            {
                var cwd = command.WorkingDirectory ?? rootDir;

                // Add zero width space
                var commandName = command.Name += "\u200B";
                SetDynamicTaskName(commandName);

                var task = new TaskRunnerNode(commandName, true)
                {
                    Command = new TaskRunnerCommand(cwd, "dotnet", $"flubu {command.Name}"),
                    Description = $"dotnet flubu {command.Name}"
                };

                tasks.Children.Add(task);
            }

            return root;
        }

        public void SetDynamicTaskName(string dynamicName)
        {
            _dynamicNames.Remove(dynamicName);
            _dynamicNames.Add(dynamicName);
        }

        public string GetDynamicName(string name)
        {
            IEqualityComparer<string> comparer = new TrimmingStringComparer('\u200B');
            return _dynamicNames.FirstOrDefault(x => comparer.Equals(name, x));
        }
    }
}