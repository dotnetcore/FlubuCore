using FlubuCore;
using FlubuCore.Context;
using FlubuCore.Services;
using Moq;
using Xunit;

namespace Flubu.Tests.Tasks
{
    public class BaseTaskUnitTests : TaskUnitTestBase
    {
        private readonly SimpleTask _task;

        public BaseTaskUnitTests()
        {
            var flubuEnviromentService = new Mock<IFlubuEnviromentService>();
            _task = new SimpleTask(flubuEnviromentService.Object);
        }

        [Fact]
        public void FromArgument_StringValueFromArgumentToStringParameter_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                {"s", "value from arg"}
            });

            _task.ForMember(x => x.AddPath("default vaue"), "-s");
            _task.Execute(Context.Object);
            Assert.Equal("value from arg", _task.Path);
        }

        [Fact]
        public void FromArgument_IntValueFromArgumentToIntParameter_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                {"l", "2"}
            });

            _task.ForMember(x => x.SetLevel(0), "-l", "help bla bla");

            _task.Execute(Context.Object);
            Assert.Equal(2, _task.Level);
        }

        [Fact]
        public void FromArgument_DefaultValue_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                {"l", "2"},
            });

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

            _task.ForMember(x => x.AddPath("default value"), "-s");
            _task.Execute(Context.Object);
            Assert.Equal("default value", _task.Path);
        }

        [Fact]
        public void FromArgument_MethodWithNoParametersIncludeWithBoolArgument_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                {"l", "true"},
            });

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

            _task.ForMember(x => x.NoParameter(), "-l");
            _task.Execute(Context.Object);
            Assert.True(_task.BoolValue);
        }

        [Fact]
        public void FromArgument_MethodWithNoParametersExcludeWithBoolArgument_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                {"l", "false"},
            });

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

            _task.ForMember(x => x.NoParameter(), "-l");
            _task.Execute(Context.Object);
            Assert.True(_task.BoolValue);
        }

        [Fact]
        public void FromArgument_MethodWithNoParametersIncludeByDefault_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                {"s", "some arg"},
            });

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

            _task.ForMember(x => x.NoParameter(), "-l");
            _task.Execute(Context.Object);
            Assert.True(_task.BoolValue);
        }

        [Fact]
        public void FromArgument_MethodWithNoParametersExcludeByDefault_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                {"s", "some arg"},
            });

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

            _task.ForMember(x => x.NoParameter(), "-l", includeParameterlessMethodByDefault: false);
            _task.Execute(Context.Object);
            Assert.False(_task.BoolValue);
        }

        [Fact]
        public void FromArgument_MethodWithNoParametersArgumentNotBoolValueIncludedByDefault_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                {"l", "some arg"},
            });

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

            _task.ForMember(x => x.NoParameter(), "-l");
            _task.Execute(Context.Object);
            Assert.True(_task.BoolValue);
        }

        [Fact]
        public void FromArgument_MethodWithNoParametersArgumentNotBoolValueExcludedByDefault_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                {"l", "some arg"},
            });

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

            _task.ForMember(x => x.NoParameter(), "-l", includeParameterlessMethodByDefault: false);
            _task.Execute(Context.Object);
            Assert.False(_task.BoolValue);
        }
        [Fact]
        public void FromArgument_StringValueFromArgumentToIntParameter_ThrowsTaskExecutionException()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                {"l", "abc"}
            });

            _task.ForMember(x => x.SetLevel(-1), "-l", "help bla bla");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal(
                "Parameter 'Int32 level' in method 'SetLevel' can not be modified with value 'abc' from argument '-l'.",
                ex.Message);
            Assert.Equal(21, ex.ErrorCode);
        }

        [Fact]
        public void FromArgument_DisabledOnExecute_ThrowsTaskExecutionException()
        {
            _task.ForMember(x => x.Execute(Context.Object), "-t");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal("ForMember is not allowed on method 'Execute'.", ex.Message);
            Assert.Equal(20, ex.ErrorCode);
        }

        [Fact]
        public void FromArgument_DisabledOnExecuteAsync_ThrowsTaskExecutionException()
        {
            _task.ForMember(x => x.ExecuteAsync(Context.Object), "-t");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal(20, ex.ErrorCode);
        }

        [Fact]
        public void FromArgument_DisabledOnExecuteVoid_ThrowsTaskExecutionException()
        {
            _task.ForMember(x => x.ExecuteVoid(Context.Object), "-t");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal(20, ex.ErrorCode);
        }

        [Fact]
        public void FromArgument_DisabledOnExecuteVoidAsync_ThrowsTaskExecutionException()
        {
            _task.ForMember(x => x.ExecuteVoidAsync(Context.Object), "-t");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal(20, ex.ErrorCode);
        }

        [Fact]
        public void FromArgument_DisabledOnFromArgument_ThrowsTaskExecutionException()
        {
            _task.ForMember(x => x.ForMember(null, "t", "test", true), "-t");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal(20, ex.ErrorCode);
        }
    }
}
