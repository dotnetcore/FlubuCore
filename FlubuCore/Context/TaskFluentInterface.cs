using FlubuCore.Tasks.FileSystem;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Context
{
    public class TaskFluentInterface
    {
        private readonly TaskContext _context;

        public TaskFluentInterface(TaskContext context)
        {
            _context = context;
        }

        public RunProgramTask RunProgramTask(string programToExecute)
        {
            return _context.CreateTask<RunProgramTask>(programToExecute);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CopyDirectoryStructureTask" /> class
        ///     using a specified source and destination path and an indicator whether to overwrite existing files.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="overwriteExisting">if set to <c>true</c> the task will overwrite existing destination files.</param>
        public CopyDirectoryStructureTask CopyDirectoryStructureTask(string sourcePath, string destinationPath, bool overwriteExisting)
        {
            return _context.CreateTask<CopyDirectoryStructureTask>(sourcePath, destinationPath, overwriteExisting);
        }
    }
}
