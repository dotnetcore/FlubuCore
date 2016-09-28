using FlubuCore.Targeting;
using FlubuCore.Tasks.NetCore;

namespace FlubuCore.Extensions
{
    public static class BuildExtensions
    {
        public static ITarget PackageProject(this ITarget target, string projectName)
        {
            target.AddTask(
                new ExecuteDotnetTask(StandardDotnetCommands.Restore)
                    .WithArguments(projectName),
                new ExecuteDotnetTask(StandardDotnetCommands.Publish)
                    .WithArguments(projectName));

            return target;
        }
    }
}
