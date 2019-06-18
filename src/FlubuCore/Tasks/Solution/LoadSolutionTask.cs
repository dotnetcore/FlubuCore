using FlubuCore.Context;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;

namespace FlubuCore.Tasks.Solution
{
    public class LoadSolutionTask : TaskBase<VSSolution, LoadSolutionTask>
    {
        private string _description;

        /// <summary>
        /// Task load's solution to <see cref="BuildPropertiesSession"/> solution file name is retieved from <see cref="BuildPropertiesSession"/>
        /// </summary>
        public LoadSolutionTask()
        {
        }

        public LoadSolutionTask(string solutionFile)
        {
            SolutionFile = solutionFile;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Loads solution {SolutionFile}";
                }

                return _description;
            }

            set { _description = value; }
        }

        public string SolutionFile { get; private set; }

        protected override VSSolution DoExecute(ITaskContextInternal context)
        {
            if (string.IsNullOrEmpty(SolutionFile))
            {
                SolutionFile = context.Properties.Get<string>(BuildProps.SolutionFileName, null);
                if (string.IsNullOrEmpty(SolutionFile))
                {
                    throw new TaskExecutionException($"Solution file name not set. Set it through fluent interface or build property 'BuildProps.{nameof(BuildProps.SolutionFileName)}'", 0);
                }

                var sol = context.Properties.TryGet<VSSolution>(BuildProps.Solution);
                if (sol != null)
                {
                    return sol;
                }
            }

            DoLogInfo($"Load solution {SolutionFile} information");

            VSSolution solution = VSSolution.Load(SolutionFile);
            context.Properties.Set(BuildProps.Solution, solution);
            solution.LoadProjects();
            return solution;
        }
    }
}
