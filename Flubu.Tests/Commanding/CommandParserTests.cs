using DotNet.Cli.Flubu.Commanding;
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

            Assert.Equal(value, res.MainCommands[0]);
            Assert.Empty(res.RemainingCommands);
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
            Assert.Equal(2, res.RemainingCommands.Count);
            Assert.False(res.Help);
        }

        [Theory]
        [InlineData("-s")]
        [InlineData("--script")]
        public void ParseScript(string value)
        {
            var res = _parser.Parse(new[] { "test", value, "b.cs"});

            Assert.Equal("b.cs", res.Script);
			Assert.Equal(0, res.ScriptArguments.Count);
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
		    Assert.Equal(1, res.ScriptArguments.Count);
		    Assert.Equal("pass=qq", res.ScriptArguments["Password"]);
	    }

	    [Fact]
	    public void GetDefaultScriptArgumentWhenKeyNotFound()
	    {
		    var res = _parser.Parse(new[] { "test", "-s", "b.cs", "--Password=pass=qq" });
		    Assert.Equal(1, res.ScriptArguments.Count);
		    Assert.Equal(null, res.ScriptArguments["NonExist"]);
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