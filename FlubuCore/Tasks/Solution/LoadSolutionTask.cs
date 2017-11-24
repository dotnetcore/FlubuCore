using FlubuCore.Context;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;

namespace FlubuCore.Tasks.Solution
{
    public class LoadSolutionTask : TaskBase<int, LoadSolutionTask>
    {
        /// <summary>
        /// Task load's solution to <see cref="TaskContextSession"/> solution file name is retieved from <see cref="TaskContextSession"/>
        /// </summary>
        public LoadSolutionTask()
        {
        }

        public LoadSolutionTask(string solutionFile)
        {
            SolutionFile = solutionFile;
        }

        public string SolutionFile { get; private set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            context.LogInfo($"Load solution {SolutionFile} properties");

            if (string.IsNullOrEmpty(SolutionFile))
            {
                SolutionFile = context.Properties.Get<string>(BuildProps.SolutionFileName);
                if (string.IsNullOrEmpty(SolutionFile))
                {
                    throw new TaskExecutionException("Solution file name not set", 0);
                }
            }

            VSSolution solution = VSSolution.Load(SolutionFile);
            context.Properties.Set(BuildProps.Solution, solution);
            solution.LoadProjects();
            return 0;
        }
    }
}
