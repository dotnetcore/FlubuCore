using FlubuCore.Context;
using FlubuCore.IO.Wrappers;
using FlubuCore.Tasks;

namespace FlubuCore.Tests
{
    public class SimpleTask : TaskBase<int, SimpleTask>
    {
        private readonly IFileWrapper _flubuEnviromentService;

        public SimpleTask(IFileWrapper flubuEnviromentService)
        {
            _flubuEnviromentService = flubuEnviromentService;
        }

        protected override string Description { get; set; }

        public string Path { get; set; }

        public string Path2 { get; set; }

        public string Path3 { get; set; }

        public int Level { get; set; }

        public bool BoolValue { get; set; }

        public SimpleTask AddPath(string path)
        {
            Path = path;
            return this;
        }

        public SimpleTask AddPath2(string path)
        {
            Path2 = path;
            return this;
        }

        public SimpleTask AddPath3(string path)
        {
            Path3 = path;
            return this;
        }

        public SimpleTask SetLevel(int level)
        {
            Level = level;
            return this;
        }

        public SimpleTask NoParameter()
        {
            BoolValue = true;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _flubuEnviromentService.Exists("test");
            return 0;
        }
    }
}
