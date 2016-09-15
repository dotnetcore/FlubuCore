using flubu.Commanding;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Xunit;

namespace flubu.tests.Commanding
{
    public class CommandParserTests
    {
        private ILoggerFactory _logFactory = new LoggerFactory();
        private FlubuCommandParser _parser;

        public CommandParserTests()
        {
            _parser = new FlubuCommandParser(new CommandLineApplication(false),
                _logFactory.CreateLogger<FlubuCommandParser>());
        }

        [Fact]
        public void ParseNull()
        {
            Command res = _parser.Parse(null);

            Assert.Equal("Debug", res.Config);
        }

        [Fact]
        public void ParseEmpty()
        {
            Command res = _parser.Parse(new string[0]);

            Assert.Equal("Debug", res.Config);
        }
    }
}
