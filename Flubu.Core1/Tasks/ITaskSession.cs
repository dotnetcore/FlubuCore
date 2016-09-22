using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flubu.Targeting;

namespace Flubu.Tasks
{
    public interface ITaskSession : ITaskContext
    {
        bool HasFailed { get; }

        TargetTree TargetTree { get; }

        void Start(Action<ITaskSession> onFinishDo);

        void Complete();
    }
}
