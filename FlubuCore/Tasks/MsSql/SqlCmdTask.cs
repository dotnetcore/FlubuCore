using System.Collections.Generic;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.MsSql
{
    /// <inheritdoc />
    /// <summary>
    /// Execute SQL script file with sqlcmd.exe
    /// </summary>
    public class SqlCmdTask : TaskBase<int>
    {
        private const string DefaultSqlCmdExe =
            @"C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\130\Tools\Binn\SQLCMD.EXE";

        private readonly List<string> _arguments = new List<string>();
        private string _workingFolder;
        private readonly List<string> _sqlCmdExePaths = new List<string>();
        private string _output;
        private string _errorOutput;

        /// <inheritdoc />
        public SqlCmdTask(string sqlFileName)
        {
            SqlFile = sqlFileName;
            _sqlCmdExePaths.Add(DefaultSqlCmdExe);
        }

        /// <summary>
        /// Dotnet command to be executed.
        /// </summary>
        public string SqlFile { get;  }

        /// <summary>
        /// Add's Argument to the dotnet see <c>Command</c>
        /// </summary>
        /// <param name="arg">Argument to be added</param>
        /// <returns></returns>
        public SqlCmdTask WithArguments(string arg)
        {
            _arguments.Add(arg);
            return this;
        }

        /// <summary>
        /// Add arguments to the sqlcmd executable. See <c>Command</c>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public SqlCmdTask WithArguments(params string[] args)
        {
            _arguments.AddRange(args);
            return this;
        }

        /// <summary>
        /// Working folder of the sqlcmd command.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public SqlCmdTask WorkingFolder(string folder)
        {
            _workingFolder = folder;
            return this;
        }

        /// <summary>
        /// Add another full path to the sqlcmd executable. First one that is found will be used.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public SqlCmdTask SqlCmdExecutable(string fullPath)
        {
            _sqlCmdExePaths.Add(fullPath);
            return this;
        }

        /// <summary>
        /// Return output of the sqlcmd command.
        /// </summary>
        /// <returns></returns>
        public string GetOutput()
        {
            return _output;
        }

        /// <summary>
        /// Return output of the sqlcmd command.
        /// </summary>
        /// <returns></returns>
        public string GetErrorOutput()
        {
            return _errorOutput;
        }

        /// <inheritdoc />
        protected override int DoExecute(ITaskContextInternal context)
        {
            string program = context.Properties.GetSqlCmdExecutable();

            if (!string.IsNullOrEmpty(program))
                _sqlCmdExePaths.Add(program);

            program = FindExecutable();

            if (string.IsNullOrEmpty(program))
            {
                context.Fail("SqlCmd executable not found!", -1);
                return -1;
            }

            IRunProgramTask task = context.Tasks().RunProgramTask(program);

            task
                .WithArguments(SqlFile)
                .WithArguments(_arguments.ToArray())
                .CaptureErrorOutput()
                .CaptureOutput()
                .WorkingFolder(_workingFolder)
                .ExecuteVoid(context);

            _output = task.GetOutput();
            _errorOutput = task.GetErrorOutput();

            return 0;
        }

        private string FindExecutable()
        {
            foreach (string exePath in _sqlCmdExePaths)
            {
                if (File.Exists(exePath))
                    return exePath;
            }

            return null;
        }
    }
}