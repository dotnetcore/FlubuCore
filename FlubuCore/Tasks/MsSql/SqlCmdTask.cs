using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.MsSql
{
    /// <summary>
    /// Execute SQL script file with sqlcmd.exe
    /// </summary>
    public class SqlCmdTask : ExternalProcessTaskBase<int, SqlCmdTask>
    {
        private const string DefaultSqlCmdExe =
            @"C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\130\Tools\Binn\SQLCMD.EXE";

        private readonly List<string> _sqlCmdExePaths = new List<string>();
        private readonly List<string> _sqlFiles = new List<string>();
        private string _output;
        private string _errorOutput;
        private string _description;

        public SqlCmdTask(List<string> sqlFiles)
        {
            _sqlFiles.AddRange(sqlFiles);
            _sqlCmdExePaths.Add(DefaultSqlCmdExe);
        }

        protected override string Description
        {
            get => string.IsNullOrEmpty(_description)
                ? $"Executes sql scripts: {string.Join(",", _sqlFiles)}"
                : _description;
            set => _description = value;
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

        /// <summary>
        /// Connect to the specified SQL server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SqlCmdTask UseServer(string server, string userName, string password)
        {
            Server(server)
                .UserName(userName)
                .Password(password);

            return this;
        }

        /// <summary>
        /// Connect to the specified SQL server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public SqlCmdTask UseServer(string server, string userName, string password, string database)
        {
            UseServer(server, userName, password)
                .Database(database);

            return this;
        }

        /// <summary>
        /// Connect to server.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public SqlCmdTask Server(string server)
        {
            WithArgumentsValueRequired("-S", server);
            return this;
        }

        /// <summary>
        /// Use userName when connecting to the DB.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public SqlCmdTask UserName(string userName)
        {
            WithArgumentsValueRequired("-U", userName);
            return this;
        }

        /// <summary>
        /// Use password when connecting to the DB.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public SqlCmdTask Password(string password)
        {
            WithArgumentsValueRequired("-P", password, true);
            return this;
        }

        /// <summary>
        /// Use trusted connection when connecting to the DB.
        /// </summary>
        /// <returns></returns>
        public SqlCmdTask TrustedConnection()
        {
            WithArguments("-E");
            return this;
        }

        /// <summary>
        /// Use database name.
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public SqlCmdTask Database(string database)
        {
            WithArgumentsValueRequired("-d", database);
            return this;
        }

        /// <summary>
        /// Force that file is in UTF8 encoding. Skip auto detection.
        /// </summary>
        /// <returns></returns>
        public SqlCmdTask ForceUtf8()
        {
            WithArguments("-f", "65001");
            return this;
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

            if (_sqlFiles.Count <= 0)
            {
                context.Fail("At least one file must be specified.", -1);
                return -1;
            }

            foreach (string file in _sqlFiles)
            {
                IRunProgramTask task = context.Tasks().RunProgramTask(program);

                if (NoOutputLog)
                    task.DoNotLogOutput();

                if (DoNotLog)
                    task.NoLog();

                var args = ValidateAndGetArgumentsFlat();

                task
                    .WithArguments("-i")
                    .WithArguments(file);
                foreach (var arg in args)
                {
                    task.WithArguments(arg.arg, arg.maskArg);
                }

                task
                    .WithArguments(args.Select(x => x.arg).ToArray())
                    .CaptureErrorOutput()
                    .CaptureOutput()
                    .WorkingFolder(ExecuteWorkingFolder)
                    .ExecuteVoid(context);

                _output = task.GetOutput();
                _errorOutput = task.GetErrorOutput();
            }

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