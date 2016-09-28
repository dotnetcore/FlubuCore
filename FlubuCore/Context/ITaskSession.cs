using System;
using FlubuCore.Targeting;

namespace FlubuCore.Context
{
    public interface ITaskSession : ITaskContext
    {
        bool HasFailed { get; }

        TargetTree TargetTree { get; }

        void Start(Action<ITaskSession> onFinishDo);

        void Complete();
    }
}
