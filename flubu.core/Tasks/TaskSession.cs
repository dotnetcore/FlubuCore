using System;
using System.Diagnostics;
using Flubu.Scripting;
using Flubu.Targeting;

namespace Flubu.Tasks
{
    public class TaskSession : TaskContext, ITaskSession
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        private bool disposed;

        private Action<ITaskSession> onFinishDo;

        public TaskSession(CommandArguments args)
            : base(args)
        {
            HasFailed = true;
        }

        public TaskSession(CommandArguments args, TargetTree targetTree)
            : base(args)
        {
            HasFailed = true;
            TargetTree = targetTree;
        }

        public TargetTree TargetTree { get; }

        public bool HasFailed { get; private set; }

        public void Start(Action<ITaskSession> onFinishDo)
        {
            this.onFinishDo = onFinishDo;
            HasFailed = true;

            stopwatch.Start();
        }

        /// <summary>
        ///     Marks the runner as having completed its work successfully. This is the last method
        ///     that should be called on the runner before it gets disposed.
        /// </summary>
        public void Complete()
        {
            HasFailed = false;
        }

        public void Reset()
        {
            TargetTree.ResetTargetExecutionInfo();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    stopwatch.Stop();

                    onFinishDo?.Invoke(this);
                }

                disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}