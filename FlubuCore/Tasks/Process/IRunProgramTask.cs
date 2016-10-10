using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Tasks.Process
{
    public interface IRunProgramTask : ITask
    {
        IRunProgramTask WithArguments(string arg);

        IRunProgramTask WithArguments(params string[] args);

        IRunProgramTask WorkingFolder(string folder);
    }
}
