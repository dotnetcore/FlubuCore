using flubu.Scripting;
using flubu.Targeting;
using System;
using System.Diagnostics;

namespace flubu
{
    public interface ITaskSession : ITaskContext
    {
        bool HasFailed { get; }
        TargetTree TargetTree { get; }

        void Start(Action<ITaskSession> onFinishDo);

        void Complete();
    }

    public class TaskSession : TaskContext, ITaskSession
    {
        public TaskSession(CommandArguments args) : base(args)
        {
            hasFailed = true;
        }

        public TaskSession(CommandArguments args, TargetTree targetTree)
            : base(args)
        {
            hasFailed = true;
            this.TargetTree = targetTree;
        }

        private Stopwatch _stopwatch = new Stopwatch();

        public TargetTree TargetTree { get; }

        public bool HasFailed
        {
            get { return hasFailed; }
        }

        public void Start(Action<ITaskSession> onFinishDo)
        {
            this.onFinishDo = onFinishDo;
            hasFailed = true;

            _stopwatch.Start();
        }

        public void Reset()
        {
            TargetTree.ResetTargetExecutionInfo();
        }

        /// <summary>
        /// Marks the runner as having completed its work successfully. This is the last method
        /// that should be called on the runner before it gets disposed.
        /// </summary>
        public void Complete()
        {
            hasFailed = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                if (disposing)
                {
                    _stopwatch.Stop();

                    onFinishDo?.Invoke(this);
                }

                disposed = true;
            }

            base.Dispose(disposing);
        }

        private bool disposed;
        private bool hasFailed;
        private Action<ITaskSession> onFinishDo;
    }
}