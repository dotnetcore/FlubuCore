using System;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    public class BaseTaskUnitTests : TaskUnitTestBase
    {
        private readonly SimpleTask _task;

        public BaseTaskUnitTests()
        {
            var fileWrapper = new Mock<IFileWrapper>();
            _task = new SimpleTask(fileWrapper.Object);
        }

        [Fact]
        public void ForMember_StringValueForMemberToStringParameter_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "value from arg" }
            });

            _task.ForMember(x => x.AddPath("default vaue"), "-s");
            _task.Execute(Context.Object);
            Assert.Equal("value from arg", _task.Path);
            Assert.Equal("-s", _task.ArgumentHelp[0].argumentKey);
            Assert.Equal("Pass argument with key '-s' to method 'AddPath'. Default value: '\"default vaue\"'.", _task.ArgumentHelp[0].help);
        }

        [Fact]
        public void ForMember_IntValueForMemberToIntParameter_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "l", "2" }
            });

            _task.ForMember(x => x.SetLevel(0), "-l", "help bla bla");

            _task.Execute(Context.Object);
            Assert.Equal(2, _task.Level);
            Assert.Equal("-l", _task.ArgumentHelp[0].argumentKey);
            Assert.Equal("help bla bla", _task.ArgumentHelp[0].help);
        }

        [Fact]
        public void ForMember_SameArgumentKeyOnMoreMembers_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "value from arg" }
            });

            _task.ForMember(x => x.AddPath("default vaue"), "-s", "help");
            _task.ForMember(x => x.AddPath2("default vaue"), "-s", "help2");
            _task.ForMember(x => x.AddPath3("default vaue"), "-s", "help3");
            _task.Execute(Context.Object);
            Assert.Equal("value from arg", _task.Path);
            Assert.Equal("value from arg", _task.Path2);
            Assert.Equal("value from arg", _task.Path3);
            Assert.Equal("-s", _task.ArgumentHelp[0].argumentKey);
            Assert.Equal("help", _task.ArgumentHelp[0].help);
            Assert.Equal("-s", _task.ArgumentHelp[1].argumentKey);
            Assert.Equal("help2", _task.ArgumentHelp[1].help);
            Assert.Equal("-s", _task.ArgumentHelp[2].argumentKey);
            Assert.Equal("help3", _task.ArgumentHelp[2].help);
        }

        [Fact]
        public void ForMember_MethodWithNoParametersIncludeWithBoolArgument_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "l", "true" },
            });

            _task.ForMember(x => x.NoParameter(), "-l");
            _task.Execute(Context.Object);
            Assert.True(_task.BoolValue);
        }

        [Fact]
        public void ForMember_MethodWithNoParametersExcludeWithBoolArgument_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "l", "false" },
            });

            _task.ForMember(x => x.NoParameter(), "-l", "test", true);
            _task.Execute(Context.Object);
            Assert.False(_task.BoolValue);
        }

        [Fact]
        public void ForMember_MethodWithNoParametersIncludeByDefault_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "some arg" },
            });

            _task.ForMember(x => x.NoParameter(), "-l");
            _task.Execute(Context.Object);
            Assert.True(_task.BoolValue);
        }

        [Fact]
        public void ForMember_MethodWithNoParametersExcludeByDefault_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "some arg" },
            });

            _task.ForMember(x => x.NoParameter(), "-l", includeParameterlessMethodByDefault: false);
            _task.Execute(Context.Object);
            Assert.False(_task.BoolValue);
        }

        [Fact]
        public void ForMember_MethodWithNoParametersArgumentNotBoolValueIncludedByDefault_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "l", "some arg" },
            });

            _task.ForMember(x => x.NoParameter(), "-l");
            _task.Execute(Context.Object);
            Assert.True(_task.BoolValue);
        }

        [Fact]
        public void ForMember_MethodWithNoParametersArgumentNotBoolValueExcludedByDefault_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "l", "some arg" },
            });

            _task.ForMember(x => x.NoParameter(), "-l", includeParameterlessMethodByDefault: false);
            _task.Execute(Context.Object);
            Assert.False(_task.BoolValue);
        }

        [Fact]
        public void ForMember_StringValueForMemberToIntParameter_ThrowsTaskExecutionException()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "l", "abc" }
            });

            _task.ForMember(x => x.SetLevel(-1), "-l", "help bla bla");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal(
                "Parameter 'Int32 level' in method 'SetLevel' can not be modified with value 'abc' from argument '-l'.",
                ex.Message);
            Assert.Equal(21, ex.ErrorCode);
        }

        [Fact(Skip = "Need to fix.")]
        public void ForMember_DisabledOnExecute_ThrowsTaskExecutionException()
        {
            _task.ForMember(x => x.Execute(Context.Object), "-t");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal("ForMember is not allowed on method 'Execute'.", ex.Message);
            Assert.Equal(20, ex.ErrorCode);
        }

        [Fact]
        public void ForMember_StringValueForMemberToStringPropertyWithDefaultHelp_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "value from arg" }
            });

            _task.ForMember(x => x.Path, "-s");
            _task.Execute(Context.Object);
            Assert.Equal("value from arg", _task.Path);
            Assert.Equal("-s", _task.ArgumentHelp[0].argumentKey);
            Assert.Equal("Pass argument with key '-s' to property 'Path'.", _task.ArgumentHelp[0].help);
        }

        [Fact]
        public void ForMember_IntValueForMemberToIntPropertyWithCustomHelp_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "1" }
            });

            _task.ForMember(x => x.Level, "-s", "level help");
            _task.Execute(Context.Object);
            Assert.Equal(1, _task.Level);
            Assert.Equal("-s", _task.ArgumentHelp[0].argumentKey);
            Assert.Equal("level help", _task.ArgumentHelp[0].help);
        }

        [Fact]
        public void ForMember_BoolValueForMemberToBoolPropertyWithDefaultHelp_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "true" }
            });

            _task.ForMember(x => x.BoolValue, "-s");
            _task.Execute(Context.Object);
            Assert.True(_task.BoolValue);
            Assert.Equal("-s", _task.ArgumentHelp[0].argumentKey);
            Assert.Equal("Pass argument with key '-s' to property 'BoolValue'.", _task.ArgumentHelp[0].help);
        }

        [Fact]
        public void ForMember_StringValueForMemberToIntProperty_ThrowsTaskExecutionException()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "l", "abc" }
            });

            _task.ForMember(x => x.Level, "-l");
            var ex = Assert.Throws<ScriptException>(() => _task.Execute(Context.Object));
            Assert.Equal(
                "Property 'Level' can not be modified with value 'abc' from argument '-l'.",
                ex.Message);
        }

        [Fact]
        public void ForMember_DisabledOnExecuteAsync_ThrowsTaskExecutionException()
        {
            _task.ForMember(x => x.ExecuteAsync(Context.Object), "-t");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal(20, ex.ErrorCode);
        }

        [Fact]
        public void ForMember_DisabledOnExecuteVoidAsync_ThrowsTaskExecutionException()
        {
            _task.ForMember(x => x.ExecuteVoidAsync(Context.Object), "-t");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal(20, ex.ErrorCode);
        }

        [Fact]
        public void ForMember_DisabledOnForMember_ThrowsTaskExecutionException()
        {
            _task.ForMember(x => x.ForMember(null, "t", "test", true), "-t");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal(20, ex.ErrorCode);
        }

        [Fact]
        public void ForMember_MultipleForMembers_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "l", "2" },
            });

            _task.ForMember(x => x.AddPath("default value"), "-s");
            _task.ForMember(x => x.SetLevel(0), "-l", "help bla bla");

            _task.Execute(Context.Object);
            Assert.Equal(2, _task.Level);
            Assert.Equal("default value", _task.Path);
        }

        [Fact]
        public void ForMember_MultipleForMembers2_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "l", "2" },
                { "s", "test" },
            });

            _task.ForMember(x => x.AddPath("default value"), "-s");
            _task.ForMember(x => x.SetLevel(0), "-l", "help bla bla");

            _task.Execute(Context.Object);
            Assert.Equal(2, _task.Level);
            Assert.Equal("test", _task.Path);
        }

        [Fact]
        public void ForMember_MultipleForMembers3_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "test" },
            });

            _task.ForMember(x => x.NoParameter(), "-l", "help bla bla", false);
            _task.ForMember(x => x.AddPath("default value"), "-s");

            _task.Execute(Context.Object);
            Assert.Equal("test", _task.Path);
        }

        [Fact]
        public void ForMember_MultipleForMembers4_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "test" },
            });

            _task.ForMember(x => x.NoParameter(), "-l", "help bla bla", true);
            _task.ForMember(x => x.AddPath("default value"), "-s");

            _task.Execute(Context.Object);
            Assert.Equal("test", _task.Path);
        }

        [Fact]
        public void ForMember_MultipleForMembers5_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "test" },
            });

            _task.ForMember(x => x.NoParameter(), "-l", "help bla bla", true);
            _task.ForMember(x => x.AddPath("default value"), "-s");

            _task.Execute(Context.Object);
            Assert.Equal("test", _task.Path);
        }

        [Fact]
        public void ForMember_MultipleForMembers6_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "test" },
            });

            _task.ForMember(x => x.Path3, "-t");
            _task.ForMember(x => x.AddPath("default value"), "-s");

            _task.Execute(Context.Object);
            Assert.Equal("test", _task.Path);
        }

        [Fact]
        public void ForMember_MultipleForMembers7_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "test" },
                { "t", "test2" }
            });

            _task.ForMember(x => x.Path3, "-t");
            _task.ForMember(x => x.AddPath("default value"), "-s");

            _task.Execute(Context.Object);
            Assert.Equal("test", _task.Path);
            Assert.Equal("test2", _task.Path3);
        }

        [Fact]
        public void Retry_ConditionMeet_FailsAfter3Times()
        {
            var failTask = new FailTask();
            Assert.Throws<Exception>(() => failTask.Retry(3, condition: (c, e) => true).Execute(Context.Object));
            Assert.Equal(4, failTask.ExecutedTimes);
        }

        [Fact]
        public void Retry_NoCondition_Succesds()
        {
            var failTask = new FailTask();
            Assert.Throws<Exception>(() => failTask.Retry(3).Execute(Context.Object));
            Assert.Equal(4, failTask.ExecutedTimes);
        }

        [Fact]
        public void RetryWithDoNotFail_ConditionMeet_Succesds()
        {
            var failTask = new FailTask();
            failTask.Retry(3, condition: (c, e) => true).DoNotFailOnError().Execute(Context.Object);
            Assert.Equal(4, failTask.ExecutedTimes);
        }

        [Fact]
        public void RetryWithDoNotFail_NoCondition_Succesds()
        {
            var failTask = new FailTask();
            failTask.Retry(3).DoNotFailOnError().Execute(Context.Object);
            Assert.Equal(4, failTask.ExecutedTimes);
        }

        [Fact]
        public void Retry_ConditionNotMeet_Fails()
        {
            var failTask = new FailTask();
            Assert.Throws<Exception>(() => failTask.Retry(3, condition: (c, e) => false).Execute(Context.Object));
            Assert.Equal(1, failTask.ExecutedTimes);
        }

        [Fact]
        public void RetryWithDoNotFail_ConditionNotMeet_Fails()
        {
            var failTask = new FailTask();
            failTask.Retry(3, condition: (c, e) => false).DoNotFailOnError().Execute(Context.Object);
            Assert.Equal(1, failTask.ExecutedTimes);
        }

        [Fact]
        public async Task RetryAsync_ConditionMeet_Succesds()
        {
            var failTask = new FailTask();
            await Assert.ThrowsAsync<Exception>(async () => await failTask.Retry(3, condition: (c, e) => true).ExecuteAsync(Context.Object));
            Assert.Equal(4, failTask.ExecutedTimes);
        }

        [Fact]
        public async Task RetryWithDoNotFailAsync_ConditionMeet_Succesds()
        {
            var failTask = new FailTask();
            await failTask.Retry(3, condition: (c, e) => true).DoNotFailOnError().ExecuteAsync(Context.Object);
            Assert.Equal(4, failTask.ExecutedTimes);
        }

        [Fact]
        public async Task RetryAsync_ConditionNotMeet_Fails()
        {
            var failTask = new FailTask();
            await Assert.ThrowsAsync<Exception>(async () => await failTask.Retry(3, condition: (c, e) => false).ExecuteAsync(Context.Object));
            Assert.Equal(1, failTask.ExecutedTimes);
        }

        [Fact]
        public async Task RetryWithDoNotFailAsync_ConditionNotMeet_Fails()
        {
            var failTask = new FailTask();
            await failTask.Retry(3, condition: (c, e) => false).DoNotFailOnError().ExecuteAsync(Context.Object);
            Assert.Equal(1, failTask.ExecutedTimes);
        }

        [Fact]
        public async Task RetryAsync_NoCondition_Fails()
        {
            var failTask = new FailTask();
            await Assert.ThrowsAsync<Exception>(async () => await failTask.Retry(3).ExecuteAsync(Context.Object));
            Assert.Equal(4, failTask.ExecutedTimes);
        }

        [Fact]
        public async Task RetryWithDoNotFailAsync_NoCondition_Succeds()
        {
            var failTask = new FailTask();
            await failTask.Retry(3).DoNotFailOnError().ExecuteAsync(Context.Object);
            Assert.Equal(4, failTask.ExecutedTimes);
        }

        [Fact]
        public void DoNotFail_NoCondition_DoesNotFail()
        {
            var failTask = new FailTask();
            failTask.DoNotFailOnError().Execute(Context.Object);
            Assert.Equal(1, failTask.ExecutedTimes);
        }

        [Fact]
        public void DoNotFail_ConditionMeet_TaskDoesNotFail()
        {
            var failTask = new FailTask();
            failTask.DoNotFailOnError(condition: (c, e) => true).Execute(Context.Object);
            Assert.Equal(1, failTask.ExecutedTimes);
        }

        [Fact]
        public void DoNotFail_ConditionNotMeet_TaskFails()
        {
            var failTask = new FailTask();
            Assert.Throws<Exception>(() => failTask.DoNotFailOnError(condition: (c, e) => false).Execute(Context.Object));
            Assert.Equal(1, failTask.ExecutedTimes);
        }

        [Fact]
        public void DoNotFailWithRetry_ConditionMeet_TaskDoesNotFail()
        {
            var failTask = new FailTask();
            failTask.Retry(3).DoNotFailOnError(condition: (c, e) => true).Execute(Context.Object);
            Assert.Equal(4, failTask.ExecutedTimes);
        }

        [Fact]
        public void DoNotFailWithRetry_ConditionNotMeet_TaskFails()
        {
            var failTask = new FailTask();
            Assert.Throws<Exception>(() => failTask.Retry(3).DoNotFailOnError(condition: (c, e) => false).Execute(Context.Object));
            Assert.Equal(4, failTask.ExecutedTimes);
        }

        [Fact]
        public async Task DoNotFailAsync_NoCondition_DoesNotFail()
        {
            var failTask = new FailTask();
            await failTask.DoNotFailOnError().ExecuteAsync(Context.Object);
            Assert.Equal(1, failTask.ExecutedTimes);
        }

        [Fact]
        public async Task DoNotFailAsync_ConditionMeet_TaskDoesNotFail()
        {
            var failTask = new FailTask();
            await failTask.DoNotFailOnError(condition: (c, e) => true).ExecuteAsync(Context.Object);
            Assert.Equal(1, failTask.ExecutedTimes);
        }

        [Fact]
        public async Task DoNotFailAsync_ConditionNotMeet_TaskFails()
        {
            var failTask = new FailTask();
            await Assert.ThrowsAsync<Exception>(async () => await failTask.DoNotFailOnError(condition: (c, e) => false).ExecuteAsync(Context.Object));
            Assert.Equal(1, failTask.ExecutedTimes);
        }

        [Fact]
        public async Task DoNotFailWithRetryAsync_ConditionMeet_TaskDoesNotFail()
        {
            var failTask = new FailTask();
            await failTask.Retry(3).DoNotFailOnError(condition: (c, e) => true).ExecuteAsync(Context.Object);
            Assert.Equal(4, failTask.ExecutedTimes);
        }

        [Fact]
        public async Task DoNotFailWithRetryAsync_ConditionNotMeet_TaskFails()
        {
            var failTask = new FailTask();
            await Assert.ThrowsAsync<Exception>(async () => await failTask.Retry(3).DoNotFailOnError(condition: (c, e) => false).ExecuteAsync(Context.Object));
            Assert.Equal(4, failTask.ExecutedTimes);
        }

        [Fact]
        public void When_ConditionMeet_TaskActionsApplied()
        {
            _task.When(() => 1 == 1, t => { t.Path = "test"; });
            Assert.Equal("test", _task.Path);
        }

        [Fact]
        public void When_ConditionNotMeet_TaskActionsNotApplied()
        {
            _task.When(() => 1 == 2, t => { t.Path = "test"; });
            Assert.Null(_task.Path);
        }
    }
}
