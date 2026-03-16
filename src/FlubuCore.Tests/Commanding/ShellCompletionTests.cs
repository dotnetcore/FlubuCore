using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlubuCore.Commanding;
using FlubuCore.Context;
using FlubuCore.Infrastructure.Terminal;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using McMaster.Extensions.CommandLineUtils;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Commanding
{
    public class ShellCompletionTests
    {
        private readonly ShellCompletionProvider _provider;

        public ShellCompletionTests()
        {
            _provider = new ShellCompletionProvider();
        }

        [Fact]
        public void ParseCompletionsOption()
        {
            var cmdApp = new CommandLineApplication()
            {
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue
            };
            var parser = new FlubuCommandParser(cmdApp, new Mock<IFlubuConfigurationProvider>().Object, new Mock<IBuildScriptLocator>().Object, new Mock<IFileWrapper>().Object);

            var res = parser.Parse(new[] { "--completions", "flubu build" });

            Assert.True(res.IsCompletionMode);
            Assert.Equal("flubu build", res.CompletionInput);
        }

        [Fact]
        public void ParseSetupCompletionsOption()
        {
            var cmdApp = new CommandLineApplication()
            {
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue
            };
            var parser = new FlubuCommandParser(cmdApp, new Mock<IFlubuConfigurationProvider>().Object, new Mock<IBuildScriptLocator>().Object, new Mock<IFileWrapper>().Object);

            var res = parser.Parse(new[] { "--setup-completions", "bash" });

            Assert.True(res.IsSetupCompletionsMode);
            Assert.Equal("bash", res.SetupCompletionsShell);
        }

        [Fact]
        public void GetCompletions_NoScript_ReturnsBuiltInCommands()
        {
            var completions = _provider.GetCompletions("flubu ", null, null, null, null);

            Assert.Contains("new", completions);
            Assert.Contains("setup", completions);
        }

        [Fact]
        public void GetCompletions_OptionPrefix_ReturnsOptions()
        {
            var completions = _provider.GetCompletions("flubu --", null, null, null, null);

            Assert.Contains("--parallel", completions);
            Assert.Contains("--dryRun", completions);
            Assert.Contains("--noColor", completions);
            Assert.Contains("--nodeps", completions);
            Assert.Contains("--script", completions);
            Assert.Contains("--help", completions);
        }

        [Fact]
        public void GetCompletions_OptionPrefixFilter_FiltersCorrectly()
        {
            var completions = _provider.GetCompletions("flubu --no", null, null, null, null);

            Assert.Contains("--noColor", completions);
            Assert.Contains("--nodeps", completions);
            Assert.Contains("--noint", completions);
            Assert.DoesNotContain("--parallel", completions);
            Assert.DoesNotContain("--script", completions);
        }

        [Fact]
        public void GetCompletions_TargetPrefixFilter_FiltersCorrectly()
        {
            var completions = _provider.GetCompletions("flubu ne", null, null, null, null);

            Assert.Contains("new", completions);
            Assert.DoesNotContain("setup", completions);
        }

        [Fact]
        public void GetCompletions_EmptyInput_ReturnsBuiltInCommands()
        {
            var completions = _provider.GetCompletions(string.Empty, null, null, null, null);

            Assert.Contains("new", completions);
            Assert.Contains("setup", completions);
        }

        [Fact]
        public void GetCompletions_WithScriptProperties_IncludesScriptArgs()
        {
            var scriptProperties = new Mock<IScriptProperties>();
            var script = new Mock<IBuildScript>();
            scriptProperties.Setup(x => x.GetPropertiesHints(script.Object))
                .Returns(new List<Hint>
                {
                    new Hint { Name = "-configuration" },
                    new Hint { Name = "-version" },
                });

            var completions = _provider.GetCompletions("flubu -", null, script.Object, scriptProperties.Object, null);

            Assert.Contains("--parallel", completions);
            Assert.Contains("-configuration", completions);
            Assert.Contains("-version", completions);
        }

        [Theory]
        [InlineData("bash")]
        [InlineData("zsh")]
        [InlineData("pwsh")]
        [InlineData("powershell")]
        [InlineData("fish")]
        public void WriteSetupScript_SupportedShells_WritesOutput(string shell)
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            try
            {
                ShellCompletionProvider.WriteSetupScript(shell);

                var output = writer.ToString();
                Assert.NotEmpty(output);
                Assert.Contains("flubu", output);
                Assert.Contains("completions", output);
            }
            finally
            {
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            }
        }

        [Fact]
        public void WriteSetupScript_UnsupportedShell_WritesToStderr()
        {
            var errorWriter = new StringWriter();
            Console.SetError(errorWriter);

            try
            {
                ShellCompletionProvider.WriteSetupScript("unsupported");

                var errorOutput = errorWriter.ToString();
                Assert.Contains("Unsupported shell", errorOutput);
            }
            finally
            {
                Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
            }
        }

        [Fact]
        public void WriteSetupScript_Bash_ContainsCompgenAndComplete()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            try
            {
                ShellCompletionProvider.WriteSetupScript("bash");

                var output = writer.ToString();
                Assert.Contains("compgen", output);
                Assert.Contains("complete", output);
                Assert.Contains("COMP_LINE", output);
            }
            finally
            {
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            }
        }

        [Fact]
        public void WriteSetupScript_Pwsh_ContainsRegisterArgumentCompleter()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            try
            {
                ShellCompletionProvider.WriteSetupScript("pwsh");

                var output = writer.ToString();
                Assert.Contains("Register-ArgumentCompleter", output);
                Assert.Contains("CompletionResult", output);
            }
            finally
            {
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            }
        }
    }
}
