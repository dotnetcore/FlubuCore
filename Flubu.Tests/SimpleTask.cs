using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Services;
using FlubuCore.Tasks;

namespace Flubu.Tests
{
    public class SimpleTask : TaskBase<int, SimpleTask>
    {
        private IFlubuEnviromentService _flubuEnviromentService;
        public SimpleTask(IFlubuEnviromentService flubuEnviromentService)
        {
            _flubuEnviromentService = flubuEnviromentService;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _flubuEnviromentService.ListAvailableMSBuildToolsVersions();
            return 0;
        }
    }
}
