using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Scripting;

namespace FlubuCore.WebApi.Tests
{
    class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
            
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
            session.CreateTarget("SuccesfullTarget").Do(SuccesfullTarget);

            session.CreateTarget("FailedTarget").Do(FailedTarget);
        }

        public void SuccesfullTarget(ITaskContext session)
        {
            Console.WriteLine("SuccesfullTarget");
        }

        public void FailedTarget(ITaskContext session)
        {
            throw new TaskExecutionException("Error message", 5);
        }
    }
}
