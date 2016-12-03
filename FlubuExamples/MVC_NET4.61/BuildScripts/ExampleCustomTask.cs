using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flubu;
using Flubu.Builds;
using FlubuCore.Context;
using FlubuCore.Tasks;

namespace BuildScripts
{
    class ExampleCustomTask : TaskBase<>
    {
        protected override void DoExecute(ITaskContext context)
        {
            context.WriteInfo("You can write any custom task for flubu.");
        }

        public override string Description
        {
            get { return "Example Task."; }
        }
    }
}
