using System.Collections.Generic;
using FlubuCore.Commanding;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using McMaster.Extensions.CommandLineUtils;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Commanding
{
    public class CommandParserTests
    {
        private readonly FlubuCommandParser _parser;

        private readonly Mock<IFlubuConfigurationProvider> _flubuConfigurationProvider;

        private readonly Mock<IFileWrapper> _file;

        private readonly Mock<IBuildScriptLocator> _buildScriptLocator;

        public CommandParserTests()
        {
            _file = new Mock<IFileWrapper>();
            _buildScriptLocator = new Mock<IBuildScriptLocator>();
            _flubuConfigurationProvider = new Mock<IFlubuConfigurationProvider>();

            var cmdApp = new CommandLineApplication()
            {
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue
            };
            _parser = new FlubuCommandParser(cmdApp, _flubuConfigurationProvider.Object, _buildScriptLocator.Object, _file.Object);
        }

        [Theory]
        [InlineData("-h")]
        [InlineData("--help")]
        [InlineData("-?")]
        public void ParseHelp(string value)
        {
            var res = _parser.Parse(new[] { value });

            Assert.True(res.Help);
        }

        [Theory]
        [InlineData("compile")]
        [InlineData("test")]
        [InlineData("package")]
        public void ParseOneCommand(string value)
        {
            var res = _parser.Parse(new[] { value });

            Assert.Equal(value, res.MainCommands[0]);
            Assert.Empty(res.AdditionalOptions);
            Assert.False(res.Help);
        }

        [Theory]
        [InlineData("compile")]
        [InlineData("test")]
        [InlineData("package")]
        public void ParseMultipleCommands(string value)
        {
            var res = _parser.Parse(new[] { value, "-another", "-another1" });

            Assert.Equal(value, res.MainCommands[0]);
            Assert.Equal(2, res.ScriptArguments.Count);
            Assert.False(res.Help);
        }

        [Theory]
        [InlineData("-s")]
        [InlineData("--script")]
        public void ParseScript(string value)
        {
            var res = _parser.Parse(new[] { "test", value, "b.cs" });

            Assert.Equal("b.cs", res.Script);
            Assert.Empty(res.ScriptArguments);
        }

        [Fact]
        public void GetDefaultScriptArgumentWhenKeyNotFound()
        {
            var res = _parser.Parse(new[] { "test", "-s", "b.cs", "--Password=pass=qq" });
            Assert.Single(res.ScriptArguments);
            Assert.Null(res.ScriptArguments["NonExist"]);
        }

        [Fact]
        public void ParseEmpty()
        {
            var res = _parser.Parse(new string[0]);

            Assert.False(res.Help);
        }

        [Fact]
        public void ParseNull()
        {
            var res = _parser.Parse(null);
            Assert.False(res.Help);
        }

        [Fact]
        public void ParseScriptArguments()
        {
            var res = _parser.Parse(new[] { "test", "-s", "b.cs", "-User=Test", "--Password=pass" });
            Assert.Equal(2, res.ScriptArguments.Count);
            Assert.Equal("Test", res.ScriptArguments["User"]);
            Assert.Equal("pass", res.ScriptArguments["Password"]);
        }

        [Fact]
        public void ParseScriptArguments2()
        {
            var res = _parser.Parse(new[] { "test", "-s", "b.cs", "--Password=pass=qq" });
            Assert.Single(res.ScriptArguments);
            Assert.Equal("pass=qq", res.ScriptArguments["Password"]);
        }

        [Fact]
        public void ParseScriptArgumentsWithConfigurutarionOptions()
        {
            _flubuConfigurationProvider.Setup(x => x.GetConfiguration("flubusettings.json")).Returns(new Dictionary<string, string>() { { "option1", "value1" }, { "option2", "value2" }, { "option3", "value3" } });
            var res = _parser.Parse(new[] { "test", "-s", "b.cs", "--option3=valueFromCommandLine" });
            Assert.Equal(3, res.ScriptArguments.Count);
            Assert.Equal("value1", res.ScriptArguments["option1"]);
            Assert.Equal("value2", res.ScriptArguments["option2"]);
            Assert.Equal("valueFromCommandLine", res.ScriptArguments["option3"]);
        }
    }
}