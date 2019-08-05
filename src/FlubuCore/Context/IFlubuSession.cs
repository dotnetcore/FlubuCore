using System;
using System.Diagnostics;
using FlubuCore.Targeting;

namespace FlubuCore.Context
{
    public interface IFlubuSession : ITaskContextInternal
    {
        bool HasFailed { get; }

        bool? UnknownTarget { get; set; }

        Stopwatch BuildStopwatch { get; }

        void Start();

        void Complete();
    }
}
