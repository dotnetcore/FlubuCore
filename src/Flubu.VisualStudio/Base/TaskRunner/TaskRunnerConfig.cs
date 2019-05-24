using System.Windows.Media;
using Flubu.VisualStudio.TaskRunner;
using Microsoft.VisualStudio.TaskRunnerExplorer;

namespace Flubu.VisualStudio.Base.TaskRunner
{
    internal class TaskRunnerConfig : TaskRunnerConfigBase
    {
        private ImageSource _rootNodeIcon;

        public TaskRunnerConfig(TaskRunnerProvider provider, ITaskRunnerCommandContext context, ITaskRunnerNode hierarchy)
            : base(provider, context, hierarchy)
        {
        }

        public TaskRunnerConfig(TaskRunnerProvider provider, ITaskRunnerCommandContext context, ITaskRunnerNode hierarchy, ImageSource rootNodeIcon)
            : this(provider, context, hierarchy)
        {
            _rootNodeIcon = rootNodeIcon;
        }

        protected override ImageSource LoadRootNodeIcon()
        {
            return _rootNodeIcon;
        }
    }
}
