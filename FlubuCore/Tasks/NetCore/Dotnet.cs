namespace Flubu.Tasks.NetCore
{
    public static class Dotnet
    {
        public static ExecuteDotnetTask Build(string projectName = null, string workingFolder = null)
        {
            return new ExecuteDotnetTask(StandardDotnetCommands.Build)
                .WorkingFolder(workingFolder)
                .WithArguments(projectName);
        }

        public static ExecuteDotnetTask Restore(string projectName = null, string workingFolder = null)
        {
            return new ExecuteDotnetTask(StandardDotnetCommands.Restore)
                .WorkingFolder(workingFolder)
                .WithArguments(projectName);
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
