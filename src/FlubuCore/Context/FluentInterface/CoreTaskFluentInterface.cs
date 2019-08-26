using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Context.FluentInterface
{
    public class CoreTaskFluentInterface : ICoreTaskFluentInterface
    {
        private readonly ToolsFluentInterface _toolsFluent;
        private readonly LinuxTaskFluentInterface _linuxFluent;

        public CoreTaskFluentInterface(ILinuxTaskFluentInterface linuxFluent, IToolsFluentInterface toolsFluent)
        {
            _toolsFluent = (ToolsFluentInterface)toolsFluent;
            _linuxFluent = (LinuxTaskFluentInterface)linuxFluent;
        }

        public TaskContext Context { get; set; }

        public ExecuteDotnetTask ExecuteDotnetTask(string command)
        {
            return Context.CreateTask<ExecuteDotnetTask>(command);
        }

        public ExecuteDotnetTask ExecuteDotnetTask(StandardDotnetCommands command)
        {
            return Context.CreateTask<ExecuteDotnetTask>(command);
        }

        public UpdateNetCoreVersionTask UpdateNetCoreVersionTask(params string[] files)
        {
            var task = Context.CreateTask<UpdateNetCoreVersionTask>(string.Empty);
            task.AddFiles(files);
            return task;
        }

        public ILinuxTaskFluentInterface LinuxTasks()
        {
            _linuxFluent.Context = Context;
            return _linuxFluent;
        }

        public DotnetRestoreTask Restore(string projectName = null, string workingFolder = null)
        {
            DotnetRestoreTask ret = Context.CreateTask<DotnetRestoreTask>();

            if (!string.IsNullOrEmpty(workingFolder))
            {
                ret.WorkingFolder(workingFolder);
            }

            if (!string.IsNullOrEmpty(projectName))
            {
                ret.Project(projectName);
            }

            return ret;
        }

        public DotnetPublishTask Publish(string projectName = null, string workingFolder = null, string configuration = null)
        {
            DotnetPublishTask task = Context.CreateTask<DotnetPublishTask>();

            if (!string.IsNullOrEmpty(workingFolder))
            {
                task.WorkingFolder(workingFolder);
            }

            if (!string.IsNullOrEmpty(projectName))
            {
                task.Project(projectName);
            }

            if (!string.IsNullOrEmpty(configuration))
            {
                task.Configuration(configuration);
            }

            return task;
        }

        public DotnetBuildTask Build(string projectName = null, string workingFolder = null)
        {
            var task = Context.CreateTask<DotnetBuildTask>();

            if (!string.IsNullOrEmpty(workingFolder))
            {
                task.WorkingFolder(workingFolder);
            }

            if (!string.IsNullOrEmpty(projectName))
            {
                task.Project(projectName);
            }

            return task;
        }

        public DotnetPackTask Pack()
        {
            return Context.CreateTask<DotnetPackTask>();
        }

        public DotnetTestTask Test()
        {
            return Context.CreateTask<DotnetTestTask>();
        }

        public DotnetCleanTask Clean()
        {
            return Context.CreateTask<DotnetCleanTask>();
        }

        public DotnetMsBuildTask MsBuild()
        {
            return Context.CreateTask<DotnetMsBuildTask>();
        }

        public DotnetNugetPushTask NugetPush(string packagePath)
        {
            return Context.CreateTask<DotnetNugetPushTask>(packagePath);
        }

        public IToolsFluentInterface Tool()
        {
            _toolsFluent.Context = Context;
            return _toolsFluent;
        }

        public CoverletTask CoverletTask(string assembly)
        {
            return Context.CreateTask<CoverletTask>(assembly);
        }
    }
}
