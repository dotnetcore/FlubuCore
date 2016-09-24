using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flubu.Scripting;

namespace Flubu.Tasks
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
