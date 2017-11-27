using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore;
using FlubuCore.Context;
using FlubuCore.Services;
using Moq;
using Xunit;

namespace Flubu.Tests.Tasks
{
    public class BaseTaskUnitTests : TaskUnitTestBase
    {
        private SimpleTask _task;

        private Mock<IFlubuEnviromentService> flubuEnviromentService;

        public BaseTaskUnitTests()
        {
            flubuEnviromentService = new Mock<IFlubuEnviromentService>();
            _task = new SimpleTask(flubuEnviromentService.Object);
        }

        [Fact]
        public void FromArgument_StringValueFromArgumentToStringParameter_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                {"-s", "value from arg"}
            });

            _task.FromArgument(x => x.AddPath("default vaue"), "-s");
            _task.Execute(Context.Object);
            Assert.Equal("value from arg", _task.Path);
        }

        [Fact]
        public void FromArgument_IntValueFromArgumentToIntParameter_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "-l", "2" }
            });

            _task.FromArgument(x => x.SetLevel(0), "-l", "help bla bla");
          
            _task.Execute(Context.Object);
            Assert.Equal(2, _task.Level);
        }


        [Fact]
        public void FromArgument_DefaultValue_Succesfull()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "-l", "2" },
            });

            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>());

            _task.FromArgument(x => x.AddPath("default value"), "-s");
            _task.Execute(Context.Object);
            Assert.Equal("default value", _task.Path);
        }


        [Fact]
        public void FromArgument_StringValueFromArgumentToIntParameter_ThrowsTaskExecutionException()
        {
            Context.Setup(x => x.ScriptArgs).Returns(new DictionaryWithDefault<string, string>()
            {
                { "-l", "abc" }
            });

            _task.FromArgument(x => x.SetLevel(-1), "-l", "help bla bla");
            var ex =  Assert.Throws<TaskExecutionException>(() =>  _task.Execute(Context.Object));
            Assert.Equal("Parameter 'Int32 level' in method 'SetLevel' can not be modified with value 'abc' from argument '-l'.", ex.Message);
        }
    }
}
