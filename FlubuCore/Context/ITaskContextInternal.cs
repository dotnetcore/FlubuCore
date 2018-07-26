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

        string FlubuHelpText { get; set; }

        bool IsInteractive { get; }

        void Fail(string message, int errorCode);
    }
}
