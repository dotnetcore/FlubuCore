using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks;

namespace Flubu.Tests
{
    public class SimpleTask : TaskBase<int>
    {
        protected override int DoExecute(ITaskContextInternal context)
        {
            return 0;
        }
    }
}
