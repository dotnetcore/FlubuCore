using System;
using FlubuCore.Services;
using FlubuCore.Targeting;

namespace FlubuCore.Context
{
    public interface ITaskSession : ITaskContext
    {
        bool HasFailed { get; }

        TargetTree TargetTree { get; }

        IComponentProvider ComponentProvider { get; }

        void Start(Action<ITaskSession> onFinishDo);

        void Complete();
    }
}
