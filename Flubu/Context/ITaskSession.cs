using System;
using Flubu.Targeting;

namespace Flubu.Context
{
    public interface ITaskSession : ITaskContext
    {
        bool HasFailed { get; }

        TargetTree TargetTree { get; }

        void Start(Action<ITaskSession> onFinishDo);

        void Complete();
    }
}
