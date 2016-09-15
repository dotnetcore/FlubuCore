using flubu.Commanding;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Xunit;

namespace flubu.tests.Commanding
{
    public class CommandParserTests
    {
        private readonly ILoggerFactory _logFactory = new LoggerFactory();
        private readonly FlubuCommandParser _parser;

        public CommandParserTests()
        {
            _parser = new FlubuCommandParser(new CommandLineApplication(false));
        }

        [Fact]
        public void ParseNull()
        {
            CommandArguments res = _parser.Parse(null);

            Assert.Equal("Debug", res.Config);
            Assert.False(res.Help);
        }

        [Fact]
        public void ParseEmpty()
        {
            CommandArguments res = _parser.Parse(new string[0]);

            Assert.Equal("Debug", res.Config);
            Assert.False(res.Help);
        }

        [Theory]
        [InlineData("-h")]
        [InlineData("--help")]
        [InlineData("-?")]
        public void ParseHelp(string value)
        {
            CommandArguments res = _parser.Parse(new string[] { value });

            Assert.Null(res.Config);
            Assert.True(res.Help);
        }

        [Theory]
        [InlineData("compile")]
        [InlineData("test")]
        [InlineData("package")]
        public void ParseOneCommand(string value)
        {
            CommandArguments res = _parser.Parse(new string[] { value });

            Assert.Equal(value, res.MainCommand);
            Assert.Empty(res.RemainingCommands);
            Assert.False(res.Help);
        }

        [Theory]
        [InlineData("compile")]
        [InlineData("test")]
        [InlineData("package")]
        public void ParseMultipleCommands(string value)
        {
            CommandArguments res = _parser.Parse(new string[] { value, "another", "another1" });

            Assert.Equal(value, res.MainCommand);
            Assert.Equal(2, res.RemainingCommands.Count);
            Assert.False(res.Help);
        }

        [Theory]
        [InlineData("-s b.cs")]
        [InlineData("--script b.cs")]
        public void ParseScript(string value)
        {
            CommandArguments res = _parser.Parse(new string[] { "test", value });

            Assert.Equal("b.cs", res.Script);
        }

        [Theory]
        [InlineData("-p testp")]
        [InlineData("--project testp")]
        public void ParseProjectPath(string value)
        {
            CommandArguments res = _parser.Parse(new string[] { "test", value });

            Assert.Equal("testp", res.ProjectPath);
        }
    }
}