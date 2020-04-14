using System;
using FlubuCore.Tasks;

namespace FlubuCore.Targeting
{
    public class TargetDependency
    {
        public string TargetName { get; set; }

        public TaskExecutionMode TaskExecutionMode { get; set; }

        public bool Skipped { get; set; }
    }
}
