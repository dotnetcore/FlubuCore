using System;
using FlubuCore.Scripting;
using FlubuCore.Targeting;

namespace FlubuCore.Context
{
    public interface ITaskContextInternal : ITaskContext, IDisposable
    {
        /// <summary>
        /// The <see cref="TargetTree"/>
        /// </summary>
        TargetTree TargetTree { get; }

        CommandArguments Args { get; }

        bool IsInteractive { get; }

        void DecreaseDepth();

        void Fail(string message, int errorCode);

        void IncreaseDepth();

        void LogInfo(string message);

        void LogError(string message);
    }
}
