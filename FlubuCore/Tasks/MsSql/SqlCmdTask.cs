using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.MsSql
{
    /// <summary>
    /// Execute SQL script file with sqlcmd.exe
    /// </summary>
    public class SqlCmdTask : TaskBase<int>
    {
        private const string DefaultSqlCmdExe =
            @"C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\130\Tools\Binn\SQLCMD.EXE";

        private readonly List<string> _arguments = new List<string>();
        private string _workingFolder;
        private string _sqlCmdExe;

        public SqlCmdTask(string sqlFileName)
        {
            SqlFile = sqlFileName;
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
        /// Add's Arguments to the dotnet see <c>Command</c>
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public SqlCmdTask WithArguments(params string[] args)
        {
            _arguments.AddRange(args);
            return this;
        }

        /// <summary>
        /// Working folder of the dotnet command
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public SqlCmdTask WorkingFolder(string folder)
        {
            _workingFolder = folder;
            return this;
        }

        /// <summary>
        /// Path to the dotnet executable.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public SqlCmdTask SqlCmdExecutable(string fullPath)
        {
            _sqlCmdExe = fullPath;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            string program = _sqlCmdExe;

            if (string.IsNullOrEmpty(program))
                program = context.Properties.GetSqlCmdExecutable();

            if (string.IsNullOrEmpty(program))
                program = DefaultSqlCmdExe;

            if (string.IsNullOrEmpty(program))
            {
                context.Fail("SqlCmd executable not set!", -1);
                return -1;
            }

            IRunProgramTask task = context.Tasks().RunProgramTask(program);

            task
                .WithArguments(SqlFile)
                .WithArguments(_arguments.ToArray())
                .WorkingFolder(_workingFolder)
                .ExecuteVoid(context);

            return 0;
        }
    }
}