using System;
using FlubuCore.Scripting;
using FlubuCore.Tasks;

namespace FlubuCore.Context
{
    public interface ITaskContext : IDisposable
    {
        ITaskContextSession Properties { get; }

        CommandArguments Args { get; }

        bool IsInteractive { get; }

        void DecreaseDepth();

        void Fail(string message, int errorCode);

        void IncreaseDepth();

        void LogInfo(string message);

        void LogError(string message);

        ICoreTaskFluentInterface CoreTasks();

        ITaskFluentInterface Tasks();
    }
}
