using FlubuCore.Context;
using FlubuCore.Tasks.Process;
using Microsoft.DotNet.Cli.Utils;

namespace FlubuCore.Tasks.Testing
{
    public class OpenCoverToCoberturaTask : TaskBase<int, OpenCoverToCoberturaTask>
    {
        private readonly string _input;
        private readonly string _output;
        private string _toolPath = "./tools/tocobertura/OpenCoverToCoberturaConverter.exe";
        private string _workingFolder = ".";

        public OpenCoverToCoberturaTask(string inputFile, string outputFile)
        {
            _input = inputFile;
            _output = outputFile;
        }

        protected override string Description { get; set; }

        public OpenCoverToCoberturaTask WorkingFolder(string path)
        {
            _workingFolder = path;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            RunProgramTask task = new RunProgramTask(new CommandFactory(), _toolPath);

            task
                .WorkingFolder(_workingFolder)
                .WithArguments($"-input:{_input}", $"-output:{_output}");

            task.ExecuteVoid(context);

            return 0;
        }
    }
}
