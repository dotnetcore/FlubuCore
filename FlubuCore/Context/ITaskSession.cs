using System;
using FlubuCore.Targeting;

namespace FlubuCore.Context
{
    public interface ITaskSession : ITaskContextInternal
    {
        bool HasFailed { get; }

        void Start(Action<ITaskSession> onFinishDo);

        void Complete();
    }
}
