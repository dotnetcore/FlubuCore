using System;
using FlubuCore.Targeting;

namespace FlubuCore.Context
{
    public interface ITaskSession : ITaskContextInternal
    {
        bool HasFailed { get; }

        bool? UnknownTarget { get; set; }

        void Start();

        void Complete();
    }
}
