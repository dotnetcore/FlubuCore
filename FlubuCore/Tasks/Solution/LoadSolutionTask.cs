using FlubuCore.Context;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;

namespace FlubuCore.Tasks.Solution
{
    public class LoadSolutionTask : TaskBase
    {
        private string _solutionFile;

        /// <summary>
        /// Task load's solution to <see cref="TaskContextSession"/> solution file name is retieved from <see cref="TaskContextSession"/>
        /// </summary>
        public LoadSolutionTask()
        {
        }

        public LoadSolutionTask(string solutionFile)
        {
            _solutionFile = solutionFile;
        }

        protected override int DoExecute(ITaskContext context)
        {
            context.LogInfo($"Load solution {_solutionFile} properties");

            if (string.IsNullOrEmpty(_solutionFile))
            {
                _solutionFile = context.Properties.Get<string>(BuildProps.SolutionFileName);
                if (string.IsNullOrEmpty(_solutionFile))
                {
                    throw new TaskExecutionException("Solution file name not set", 0);
                }
            }

            VSSolution solution = VSSolution.Load(_solutionFile);
            context.Properties.Set(BuildProps.Solution, solution);
            solution.LoadProjects();
            return 0;
        }
    }
}
