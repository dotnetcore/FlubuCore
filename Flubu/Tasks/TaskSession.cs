using System;
using System.Diagnostics;
using Flubu.Scripting;
using Flubu.Targeting;
using Microsoft.Extensions.Logging;

namespace Flubu.Tasks
{
    public class TaskSession : TaskContext, ITaskSession
    {
        private readonly ILogger<TaskSession> _log;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private bool _disposed;

        private Action<ITaskSession> _onFinishDo;

        public TaskSession(ILogger<TaskSession> log, ITaskContextSession taskContextProperties, CommandArguments args)
            : base(log, taskContextProperties, args)
        {
            _log = log;
            HasFailed = true;
        }

        public TargetTree TargetTree { get; set; }

        public bool HasFailed { get; private set; }

        public void Start(Action<ITaskSession> onFinishDo)
        {
            _onFinishDo = onFinishDo;
            HasFailed = true;

            _stopwatch.Start();
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
            if (!_disposed)
            {
                if (disposing)
                {
                    _stopwatch.Stop();

                    _onFinishDo?.Invoke(this);
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}