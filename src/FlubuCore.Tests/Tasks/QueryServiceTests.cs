using System;
using FlubuCore.Tasks.Utils;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    public class QueryServiceTests : TaskUnitTestBase
    {
        private readonly ServiceStatusTask _task;

        public QueryServiceTests()
        {
            _task = new ServiceStatusTask("eventlog");
            Tasks.Setup(x => x.RunProgramTask("sc")).Returns(RunProgramTask.Object);
        }

        [Fact]
        public void QueryRunningService()
        {
            Properties.Setup(i => i.Set("eventlog.status", ServiceStatus.Running, true));
            RunProgramTask.Setup(i => i.GetOutput()).Returns(@"SERVICE_NAME: eventlog
        TYPE               : 30  WIN32
        STATE              : 4  RUNNING
                                (STOPPABLE, NOT_PAUSABLE, ACCEPTS_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0
        PID                : 2928
        FLAGS              :");

            _task.ExecuteVoid(Context.Object);

            Properties.VerifyAll();
        }

        [Fact]
        public void QueryStoppedService()
        {
            Properties.Setup(i => i.Set("eventlog.status", ServiceStatus.Stopped, true));
            RunProgramTask.Setup(i => i.GetOutput()).Returns(@"SERVICE_NAME: AppIDSvc
        TYPE               : 20  WIN32_SHARE_PROCESS
        STATE              : 1  STOPPED
        WIN32_EXIT_CODE    : 1077  (0x435)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0");

            _task.ExecuteVoid(Context.Object);

            Properties.VerifyAll();
        }

        [Fact]
        public void WaitForStopServiceTimeout()
        {
            Properties.Setup(i => i.Get<ServiceStatus>("eventlog.status", ServiceStatus.Unknown, string.Empty)).Returns(ServiceStatus.Running);
            var task = new WaitForServiceStopTask("eventlog");
            Tasks.Setup(x => x.RunProgramTask("sc")).Returns(RunProgramTask.Object);
            RunProgramTask.Setup(i => i.GetOutput()).Returns(@"SERVICE_NAME: AppIDSvc
        TYPE               : 20  WIN32_SHARE_PROCESS
        STATE              : 4  RUN
        WIN32_EXIT_CODE    : 1077  (0x435)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0");

            var res = task.Execute(Context.Object);
            Assert.Equal(1, res);
        }

        [Fact]
        public void WaitForStopService()
        {
            Properties.Setup(i => i.Get<ServiceStatus>("eventlog.status", true, string.Empty)).Returns(ServiceStatus.Stopped);
            var task = new WaitForServiceStopTask("eventlog");
            Tasks.Setup(x => x.RunProgramTask("sc")).Returns(RunProgramTask.Object);
            RunProgramTask.Setup(i => i.GetOutput()).Returns(@"SERVICE_NAME: AppIDSvc
        TYPE               : 20  WIN32_SHARE_PROCESS
        STATE              : 1  ST
        WIN32_EXIT_CODE    : 1077  (0x435)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0");

            var res = task.Execute(Context.Object);
            Assert.Equal(1, res);
        }
    }
}
