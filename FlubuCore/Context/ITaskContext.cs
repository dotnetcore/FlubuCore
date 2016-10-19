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

        TaskFluentInterface Tasks();

        T CreateTask<T>()
            where T : TaskBase;

        T CreateTask<T>(params object[] constructorArgs)
            where T : TaskBase;
    }
}
