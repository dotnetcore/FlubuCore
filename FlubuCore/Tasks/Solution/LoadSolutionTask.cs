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

        public override string Description => $"Load solution {_solutionFile} properties";

        protected override int DoExecute(ITaskContext context)
        {
            return 0;
        }
    }
}
