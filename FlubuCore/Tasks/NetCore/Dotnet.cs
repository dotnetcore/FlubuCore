using FlubuCore.Targeting;

namespace FlubuCore.Tasks.NetCore
{
    public static class Dotnet
    {
        public static ExecuteDotnetTask Build(string projectName = null, string workingFolder = null)
        {
            return new ExecuteDotnetTask(StandardDotnetCommands.Build)
                .WorkingFolder(workingFolder)
                .WithArguments(projectName);
        }

        public static ITarget Build(this ITarget target, params string[] projects)
        {
            foreach (string project in projects)
            {
                target.AddTask(Dotnet.Build(project));
            }

            return target;
        }

        public static ExecuteDotnetTask Restore(string projectName = null, string workingFolder = null)
        {
            ExecuteDotnetTask ret = new ExecuteDotnetTask(StandardDotnetCommands.Restore);

            if (!string.IsNullOrEmpty(workingFolder))
            {
                ret.WorkingFolder(workingFolder);
            }

            if (!string.IsNullOrEmpty(projectName))
            {
                ret.WithArguments(projectName);
            }

            return ret;
        }

        public static ExecuteDotnetTask Pack(string projectName = null, string workingFolder = null)
        {
            return new ExecuteDotnetTask(StandardDotnetCommands.Pack)
                .WorkingFolder(workingFolder)
                .WithArguments(projectName);
        }

        public static ExecuteDotnetTask Test(string projectName = null, string workingFolder = null)
        {
            return new ExecuteDotnetTask(StandardDotnetCommands.Test)
                .WorkingFolder(workingFolder)
                .WithArguments(projectName);
        }

        public static ExecuteDotnetTask Publish(string projectName = null, string workingFolder = null)
        {
            return new ExecuteDotnetTask(StandardDotnetCommands.Publish)
                .WorkingFolder(workingFolder)
                .WithArguments(projectName);
        }

        public static ExecuteDotnetTask Run(string projectName = null, string workingFolder = null)
        {
            return new ExecuteDotnetTask(StandardDotnetCommands.Run)
                .WorkingFolder(workingFolder)
                .WithArguments(projectName);
        }
    }
}
