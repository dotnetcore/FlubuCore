using FlubuCore.Targeting;

namespace FlubuCore.Tasks.Text
{
    public static class TextExtensions
    {
        public static ITarget MergeConfiguration(this ITarget target, string outFile, params string[] sourceFiles)
        {
            return target.AddTask(new MergeConfigurationTask(outFile, sourceFiles));
        }
    }
}
