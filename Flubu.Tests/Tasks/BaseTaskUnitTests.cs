using FlubuCore;
using FlubuCore.Context;
using FlubuCore.IO.Wrappers;
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
            Assert.Equal("Pass argument '-s' to method 'AddPath'. Default value: '\"default vaue\"'.", _task.ArgumentHelp[0].help);
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
        public void ForMember_DefaultValue_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "l", "2" },
            });

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

            _task.ForMember(x => x.AddPath("default value"), "-s");
            _task.Execute(Context.Object);
            Assert.Equal("default value", _task.Path);
        }

        [Fact]
        public void ForMember_MethodWithNoParametersIncludeWithBoolArgument_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "l", "true" },
            });

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

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

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

            _task.ForMember(x => x.NoParameter(), "-l");
            _task.Execute(Context.Object);
            Assert.True(_task.BoolValue);
        }

        [Fact]
        public void ForMember_MethodWithNoParametersIncludeByDefault_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "s", "some arg" },
            });

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

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

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

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

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

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

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

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

        [Fact]
        public void ForMember_DisabledOnExecute_ThrowsTaskExecutionException()
        {
            _task.ForMember(x => x.Execute(Context.Object), "-t");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal("ForMember is not allowed on method 'Execute'.", ex.Message);
            Assert.Equal(20, ex.ErrorCode);
        }

        [Fact]
        public void ForMember_DisabledOnExecuteAsync_ThrowsTaskExecutionException()
        {
            _task.ForMember(x => x.ExecuteAsync(Context.Object), "-t");
            var ex = Assert.Throws<TaskExecutionException>(() => _task.Execute(Context.Object));
            Assert.Equal(20, ex.ErrorCode);
        }

        [Fact]
        public void ForMember_DisabledOnExecuteVoid_ThrowsTaskExecutionException()
        {
            _task.ForMember(x => x.ExecuteVoid(Context.Object), "-t");
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
    }
}
