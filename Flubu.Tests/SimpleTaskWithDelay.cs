using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Tasks;

namespace Flubu.Tests
{
    public class SimpleTaskWithDelay : TaskBase<int, SimpleTaskWithDelay>
    {
        protected override int DoExecute(ITaskContextInternal context)
        {
            Task.Delay(1000).Wait();
            return 0;
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await Task.Delay(1000);
            return 0;
        }
    }
}
