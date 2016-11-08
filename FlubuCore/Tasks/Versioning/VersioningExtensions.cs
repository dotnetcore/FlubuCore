using FlubuCore.Targeting;

namespace FlubuCore.Tasks.Versioning
{
    public static class VersioningExtensions
    {
        public static ITarget FetchBuildVersionFromFile(this ITarget target)
        {
            return target.AddTask(new FetchBuildVersionFromFileTask());
        }

        public static ITarget FetchBuildVersionFromExternalSource(this ITarget target)
        {
            return target.AddTask(new FetchVersionFromExternalSourceTask());
        }

        public static ITarget UpdateDotnetVersion(this ITarget target, string[] projectFiles, string[] additionalProps)
        {
            return target.AddTask(
                new UpdateNetCoreVersionTask(projectFiles)
                    .AdditionalProp(additionalProps));
        }

        public static ITarget UpdateDotnetVersion(this ITarget target, params string[] projectFiles)
        {
            return target.AddTask(new UpdateNetCoreVersionTask(projectFiles));
        }
    }
}
