using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Services;
using Moq;

namespace Flubu.Tests.Tasks
{
    public class BaseTaskUnitTests
    {
        private SimpleTask _simpleTask;

        private Mock<IFlubuEnviromentService> flubuEnviromentService;

        public BaseTaskUnitTests()
        {
            _simpleTask = new SimpleTask(flubuEnviromentService.Object);
        }
    }
}
