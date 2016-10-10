using FlubuCore.Tasks.Process;

namespace FlubuCore.Services
{
    public interface IComponentProvider
    {
        IRunProgramTask CreateRunProgramTask(string programToExecute);

        IFlubuEnviromentService CreateFlubuEnviromentService();
    }
}
