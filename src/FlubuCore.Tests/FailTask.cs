using System;
using FlubuCore.Context;
using FlubuCore.Tasks;

namespace FlubuCore.Tests
{
    public class FailTask : TaskBase<int, FailTask>
    {
        private int _succedAfter = int.MaxValue;

        private int _executedCount = 0;

        protected override string Description { get; set; }

        public int ExecutedTimes => _executedCount;

        public FailTask SuccedAfter(int times)
        {
            _succedAfter = times;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_executedCount >= _succedAfter)
            {
                return 0;
            }

            _executedCount++;

            throw new Exception();
        }
    }
}
