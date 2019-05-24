using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Tasks;

namespace FlubuCore.Tests
{
    public class SimpleTaskWithDelay : TaskBase<int, SimpleTaskWithDelay>
    {
        private readonly int _delay;

        public SimpleTaskWithDelay(int delay = 3000)
        {
            _delay = delay;
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
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
