using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FlubuCore.Tasks.Process;
using Microsoft.DotNet.Cli.Utils;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Tasks
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
        public void ExecuteDotNetCommandWithFullPath()
        {
            var fileName = GetOsPlatform() == OSPlatform.Windows ? "C:\\Program Files\\dotnet\\dotnet.exe" : "/usr/bin/dotnet";
            string currentFolder = Directory.GetCurrentDirectory();
            var path = Path.GetFullPath(fileName);
            _commandFactory.Setup(i => i.Create(path, new List<string> { "--version" }, null, "Debug")).Returns(_command.Object);

            _command.Setup(i => i.CaptureStdErr()).Returns(_command.Object);
            _command.Setup(i => i.CaptureStdOut()).Returns(_command.Object);
            _command.Setup(i => i.WorkingDirectory(currentFolder)).Returns(_command.Object);
            _command.Setup(i => i.OnErrorLine(It.IsAny<Action<string>>())).Returns(_command.Object);
            _command.Setup(i => i.OnOutputLine(It.IsAny<Action<string>>())).Returns(_command.Object);
            _command.Setup(i => i.Execute()).Returns(new CommandResult(new ProcessStartInfo { Arguments = "aa" }, 0, string.Empty, string.Empty));

            RunProgramTask task = new RunProgramTask(_commandFactory.Object, fileName);

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

        [Fact]
        public void CaptureOutput()
        {
            Action<string> onOutput = null, onError = null;

            _commandFactory.Setup(i => i.Create("dotnet", new List<string>(), null, "Debug")).Returns(_command.Object);

            _command.Setup(i => i.CaptureStdErr()).Returns(_command.Object);
            _command.Setup(i => i.CaptureStdOut()).Returns(_command.Object);
            _command.Setup(i => i.WorkingDirectory(It.IsAny<string>())).Returns(_command.Object);
            _command.Setup(i => i.OnErrorLine(It.IsAny<Action<string>>())).Returns(_command.Object);
            _command.Setup(i => i.OnOutputLine(It.IsAny<Action<string>>())).Returns(_command.Object);
            _command.Setup(i => i.Execute()).Returns(new CommandResult(new ProcessStartInfo { Arguments = "aa" }, 0, string.Empty, string.Empty));

            _command.Setup(i => i.OnOutputLine(It.IsAny<Action<string>>()))
                    .Callback<Action<string>>(action => onOutput = action)
                    .Returns(_command.Object);

            _command.Setup(i => i.OnErrorLine(It.IsAny<Action<string>>()))
                    .Callback<Action<string>>(action => onError = action)
                    .Returns(_command.Object);

            var task = new RunProgramTask(_commandFactory.Object, "dotnet");

            int res = task.CaptureOutput()
                          .CaptureErrorOutput()
                          .DoNotLogOutput()
                          .Execute(Context);

            onOutput?.Invoke("output line 1");
            onOutput?.Invoke("output line 2");
            onError?.Invoke("error line 1");
            onError?.Invoke("error line 2");

            string output = task.GetOutput()?.Replace("\r", string.Empty)?.Replace("\n", string.Empty);
            string outputError = task.GetErrorOutput()?.Replace("\r", string.Empty)?.Replace("\n", string.Empty);

            Assert.Equal(0, res);
            Assert.Equal("output line 1output line 2", output);
            Assert.Equal("error line 1error line 2", outputError);
        }
    }
}
