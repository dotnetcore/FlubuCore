using FlubuCore.Context;

namespace FlubuCore.Tasks.Solution
{
    public class LoadSolutionTask : TaskBase
    {
        private readonly string _solutionFile;

        public LoadSolutionTask(string solutionFile)
        {
            _solutionFile = solutionFile;
        }

        protected override int DoExecute(ITaskContext context)
        {
            context.LogInfo($"Load solution {_solutionFile} properties");

            return 0;
        }
    }
}
