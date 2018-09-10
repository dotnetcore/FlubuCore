using System;
using FlubuCore.Context;
using FlubuCore.Tasks;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    public class DoTaskTests : TaskUnitTestBase
    {
        [Fact]
        public void NewDoTask_DefaultTaskName_IsMethodName()
        {
            var task = new DoTaskForTests(SomeMethodName);

            Assert.Equal("SomeMethodName", task.GetTaskName());
        }

        [Fact]
        public void NewDoTask_DefaultTaskNameAnonymousMethod_IsMethodName()
        {
            var task = new DoTaskForTests((c) => { return; });

            Assert.Equal("<NewDoTask_DefaultTaskNameAnonymousMethod_IsMethodName>b__1_0", task.GetTaskName());
        }

        private void SomeMethodName(ITaskContext context)
        {
        }
    }

    public class DoTaskForTests : DoTask
    {
        public DoTaskForTests(Action<ITaskContextInternal> taskAction)
            : base(taskAction)
        {
        }

        public string GetTaskName()
        {
            return TaskName;
        }
    }
}
