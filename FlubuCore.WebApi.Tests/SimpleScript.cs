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
            session.CreateTarget("test").Do(Test);
        }

        public void Test(ITaskContext session)
        {
            Console.WriteLine("Test");
        }
    }
}
