using System.Collections.Generic;

namespace FlubuCore.BuildServers.Configurations.Models.AzurePipelines.Job
{
   public class JobItem
    {
        private List<object> _steps = new List<object>();

        public string Job { get; set; }

        public Pool Pool { get; set; }

        public List<string> DependsOn { get; set; }

        public List<string> Condition { get; set; }

        public IList<object> Steps => _steps.AsReadOnly();

        public void AddStep<T>(T step)
            where T : IStep
        {
            _steps.Add(step);
        }
    }
}
