using FlubuCore.Context;
using FlubuCore.Services;
using FlubuCore.Tasks;

namespace Flubu.Tests
{
    public class SimpleTask : TaskBase<int, SimpleTask>
    {
        private readonly IFlubuEnviromentService _flubuEnviromentService;

        public SimpleTask(IFlubuEnviromentService flubuEnviromentService)
        {
            _flubuEnviromentService = flubuEnviromentService;
        }

        protected override string Description { get; set; }

        public string Path { get; set; }

        public int Level { get; set; }

        public bool BoolValue { get; set; }

        public SimpleTask AddPath(string path)
        {
            Path = path;
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
            _flubuEnviromentService.ListAvailableMSBuildToolsVersions();
            return 0;
        }
    }
}
