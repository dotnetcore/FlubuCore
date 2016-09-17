using flubu.Scripting;
using Flubu;
using System;
using System.Collections.Generic;

namespace flubu.Tasks
{
    public interface ITaskContext : IDisposable
    {
        ITaskContextProperties Properties { get; }
        CommandArguments Args { get; }

        bool IsInteractive { get; }

        void DecreaseDepth();
        void Fail(string message);
        void IncreaseDepth();
        void ResetDepth();
        void WriteMessage(TaskMessageLevel level, string message);
    }
}
