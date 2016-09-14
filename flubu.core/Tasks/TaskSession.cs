using System;
using System.Collections.Generic;
using System.Diagnostics;
using flubu.Tasks;
using flubu.Targeting;

namespace flubu
{
    public class TaskSession : TaskContext, ITaskSession
    {
        public TaskSession(ITaskContextProperties taskContextProperties, IEnumerable<string> args) : base(taskContextProperties, args)
        {
            hasFailed = true;
            buildStopwatch.Start();
        }

        public TaskSession (ITaskContextProperties taskContextProperties, IEnumerable<string> args, TargetTree targetTree)
            : base(taskContextProperties, args)
        {
            hasFailed = true;
            buildStopwatch.Start();
            this.targetTree = targetTree;
        }

        public Stopwatch BuildStopwatch
        {
            get { return buildStopwatch; }
        }

        public TargetTree TargetTree
        {
            get { return targetTree; }
        }

        public bool HasFailed
        {
            get { return hasFailed; }
        }

        public void Start(Action<ITaskSession> onFinishDo)
        {
            this.onFinishDo = onFinishDo;
            hasFailed = true;
            buildStopwatch.Start();
        }

        public void Reset()
        {
            targetTree.ResetTargetExecutionInfo();
            Properties.Clear();
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
                    buildStopwatch.Stop();

                    if (onFinishDo != null)
                        onFinishDo(this);
                }

                disposed = true;
            }

            base.Dispose(disposing);
        }

        private readonly Stopwatch buildStopwatch = new Stopwatch();
        private bool disposed;
        private bool hasFailed;
        private Action<ITaskSession> onFinishDo;
        private readonly TargetTree targetTree;
    }
}