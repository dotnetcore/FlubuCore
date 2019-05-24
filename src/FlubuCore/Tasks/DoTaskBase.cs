namespace FlubuCore.Tasks
{
    public abstract class DoTaskBase<TResult, TTask> : TaskBase<TResult, TTask>
        where TTask : class, ITask
    {
        private string _description;

        protected override string Description
        {
            get => _description;

            set => _description = value;
        }

        /// <summary>
        /// Name of the task that is displayed in help.
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public TTask SetTaskName(string taskName)
        {
            TaskName = taskName;
            return this as TTask;
        }

        /// <summary>
        /// Adds help for specified argument.
        /// </summary>
        /// <param name="argKey"></param>
        /// <param name="help"></param>
        /// <returns></returns>
        public TTask AddArgumentHelp(string argKey, string help)
        {
            ArgumentHelp.Add((argKey, help));
            return this as TTask;
        }
    }
}
