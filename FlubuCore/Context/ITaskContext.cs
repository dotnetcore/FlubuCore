using System;
using FlubuCore.Scripting;

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

        void WriteMessage(string message);
    }
}
