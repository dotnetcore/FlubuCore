using FlubuCore.Tasks.Process;

namespace FlubuCore.Infrastructure
{
    public interface IComponentProvider
    {
        IRunProgramTask CreateRunProgramTask(string programToExecute);
    }
}
