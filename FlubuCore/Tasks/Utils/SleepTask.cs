using System;
using System.Threading.Tasks;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Utils
{
    /// <inheritdoc />
    /// <summary>
    /// Sleep task. Just wait for specified period of time.
    /// </summary>
    public class SleepTask : TaskBase<int, SleepTask>
    {
        private readonly TimeSpan _delay;
        private string _description;

        /// <summary>
        /// Constructs new <see cref="SleepTask"/>
        /// </summary>
        /// <param name="delay">Delay in milliseconds</param>
        public SleepTask(int delay)
        {
            _delay = TimeSpan.FromMilliseconds(delay);
        }

        /// <summary>
        /// Constructs new <see cref="SleepTask"/>
        /// </summary>
        /// <param name="delay">Amount of time to sleep.</param>
        public SleepTask(TimeSpan delay)
        {
            _delay = delay;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Sleeps for {_delay.Milliseconds} in milliseconds";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <inheritdoc />
        protected override int DoExecute(ITaskContextInternal context)
        {
            DoLogInfo($"Sleeping for {_delay}");

            Task.Delay(_delay).Wait();

            return 0;
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await Task.Delay(_delay);

            return 0;
        }
    }
}
