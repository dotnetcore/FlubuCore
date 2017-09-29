using System;
using System.Threading.Tasks;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Utils
{
    /// <inheritdoc />
    /// <summary>
    /// Sleep task. Just wait for specified period of time.
    /// </summary>
    public class SleepTask : TaskBase<int>
    {
        private readonly TimeSpan _delay;

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

        /// <inheritdoc />
        protected override int DoExecute(ITaskContextInternal context)
        {
            context.LogInfo($"Sleeping for {_delay}");

            Task.Delay(_delay).Wait();

            return 0;
        }
    }
}
