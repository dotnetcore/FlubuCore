using Flubu.Commanding;
using Microsoft.Extensions.CommandLineUtils;
using Xunit;

namespace Flubu.Tests.Commanding
{
    public class CommandParserTests
    {
        private readonly FlubuCommandParser _parser;

        public CommandParserTests()
        {
            _parser = new FlubuCommandParser(new CommandLineApplication(false));
        }

        [Theory]
        [InlineData("-h")]
        [InlineData("--help")]
        [InlineData("-?")]
        public void ParseHelp(string value)
        {
            var res = _parser.Parse(new[] { value });

            Assert.Null(res.Config);
            Assert.True(res.Help);
        }

        [Theory]
        [InlineData("compile")]
        [InlineData("test")]
        [InlineData("package")]
        public void ParseOneCommand(string value)
        {
            var res = _parser.Parse(new[] { value });

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
            var res = _parser.Parse(new[] { value, "another", "another1" });

            Assert.Equal(value, res.MainCommand);
            Assert.Equal(2, res.RemainingCommands.Count);
            Assert.False(res.Help);
        }

        [Theory]
        [InlineData("-s")]
        [InlineData("--script")]
        public void ParseScript(string value)
        {
            var res = _parser.Parse(new[] { "test", value, "b.cs" });

            Assert.Equal("b.cs", res.Script);
        }

        [Theory]
        [InlineData("-a")]
        [InlineData("--assembly")]
        public void ParseProjectPath(string value)
        {
            var res = _parser.Parse(new[] { "test", value, "testp" });

            Assert.Equal("testp", res.ScriptAssembly);
        }

        [Fact]
        public void ParseEmpty()
        {
            var res = _parser.Parse(new string[0]);

            Assert.Equal("Debug", res.Config);
            Assert.False(res.Help);
        }

        [Fact]
        public void ParseNull()
        {
            var res = _parser.Parse(null);

            Assert.Equal("Debug", res.Config);
            Assert.False(res.Help);
        }
    }
}