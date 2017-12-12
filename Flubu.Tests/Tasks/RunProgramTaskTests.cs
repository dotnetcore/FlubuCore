using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FlubuCore.Tasks.Process;
using Microsoft.DotNet.Cli.Utils;
using Moq;
using Xunit;

namespace Flubu.Tests.Tasks
{
    [Collection(nameof(FlubuTestCollection))]
    public class RunProgramTaskTests : FlubuTestBase
    {
        private readonly FlubuTestFixture _fixture;
        private readonly Mock<ICommandFactory> _commandFactory = new Mock<ICommandFactory>();
        private readonly Mock<ICommand> _command = new Mock<ICommand>();

        public RunProgramTaskTests(FlubuTestFixture fixture)
            : base(fixture.LoggerFactory)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "OnlyWindows")]
        public void ExecuteDotNetCommandWithFullPath()
        {
            string currentFolder = Directory.GetCurrentDirectory();
            var path = Path.GetFullPath("C:\\Program Files\\dotnet\\dotnet.exe");
            _commandFactory.Setup(i => i.Create(path, new List<string> { "--version" }, null, "Debug")).Returns(_command.Object);

            _command.Setup(i => i.CaptureStdErr()).Returns(_command.Object);
            _command.Setup(i => i.CaptureStdOut()).Returns(_command.Object);
            _command.Setup(i => i.WorkingDirectory(currentFolder)).Returns(_command.Object);
            _command.Setup(i => i.OnErrorLine(It.IsAny<Action<string>>())).Returns(_command.Object);
            _command.Setup(i => i.OnOutputLine(It.IsAny<Action<string>>())).Returns(_command.Object);
            _command.Setup(i => i.Execute()).Returns(new CommandResult(new ProcessStartInfo { Arguments = "aa" }, 0, string.Empty, string.Empty));

            RunProgramTask task = new RunProgramTask(_commandFactory.Object, "C:/Program Files/dotnet/dotnet.exe");

            int res = task
                .WithArguments("--version")
                .Execute(Context);

            Assert.Equal(0, res);
        }

        [Fact]
        public void ExecuteCommand()
        {
            string currentFolder = Directory.GetCurrentDirectory();

            _commandFactory.Setup(i => i.Create("dotnet", new List<string> { "--version" }, null, "Debug")).Returns(_command.Object);

            _command.Setup(i => i.CaptureStdErr()).Returns(_command.Object);
            _command.Setup(i => i.CaptureStdOut()).Returns(_command.Object);
            _command.Setup(i => i.WorkingDirectory(currentFolder)).Returns(_command.Object);
            _command.Setup(i => i.OnErrorLine(It.IsAny<Action<string>>())).Returns(_command.Object);
            _command.Setup(i => i.OnOutputLine(It.IsAny<Action<string>>())).Returns(_command.Object);
            _command.Setup(i => i.Execute()).Returns(new CommandResult(new ProcessStartInfo { Arguments = "aa" }, 0, string.Empty, string.Empty));

            RunProgramTask task = new RunProgramTask(_commandFactory.Object, "dotnet");

            int res = task
                .WithArguments("--version")
                .Execute(Context);

            Assert.Equal(0, res);
        }

        [Fact]
        public void ResolveExecutableWithDefaultWorkingFolder()
        {
            string currentFolder = Directory.GetCurrentDirectory();
            string file = Directory.EnumerateFiles(currentFolder).FirstOrDefault();
            string command = Path.GetFileName(file);

            _commandFactory.Setup(i => i.Create(file, new List<string>(), null, "Debug")).Returns(_command.Object);

            _command.Setup(i => i.CaptureStdErr()).Returns(_command.Object);
            _command.Setup(i => i.CaptureStdOut()).Returns(_command.Object);
            _command.Setup(i => i.WorkingDirectory(currentFolder)).Returns(_command.Object);
            _command.Setup(i => i.OnErrorLine(It.IsAny<Action<string>>())).Returns(_command.Object);
            _command.Setup(i => i.OnOutputLine(It.IsAny<Action<string>>())).Returns(_command.Object);
            _command.Setup(i => i.Execute()).Returns(new CommandResult(new ProcessStartInfo { Arguments = "aa" }, 0, string.Empty, string.Empty));

            RunProgramTask task = new RunProgramTask(_commandFactory.Object, command);

            task
                .ExecuteVoid(Context);
        }

        [Fact]
        public void ResolveExecutableWithWorkingFolderSet()
        {
            string currentFolder = Directory.GetCurrentDirectory();
            string file = Directory.EnumerateFiles(currentFolder).FirstOrDefault();
            string command = Path.GetFileName(file);

            _commandFactory.Setup(i => i.Create(file, new List<string>(), null, "Debug")).Returns(_command.Object);

            _command.Setup(i => i.CaptureStdErr()).Returns(_command.Object);
            _command.Setup(i => i.CaptureStdOut()).Returns(_command.Object);
            _command.Setup(i => i.WorkingDirectory("test")).Returns(_command.Object);
            _command.Setup(i => i.OnErrorLine(It.IsAny<Action<string>>())).Returns(_command.Object);
            _command.Setup(i => i.OnOutputLine(It.IsAny<Action<string>>())).Returns(_command.Object);
            _command.Setup(i => i.Execute()).Returns(new CommandResult(new ProcessStartInfo { Arguments = "aa" }, 0, string.Empty, string.Empty));

            RunProgramTask task = new RunProgramTask(_commandFactory.Object, command);

            task
                .WorkingFolder("test")
                .ExecuteVoid(Context);
        }
    }
}
