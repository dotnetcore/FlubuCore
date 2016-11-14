using System;
using FlubuCore.Targeting;

namespace FlubuCore.Tasks.Text
{
    public static class TextExtensions
    {
        public static ITarget MergeConfiguration(this ITarget target, string outFile, params string[] sourceFiles)
        {
            return target.AddTask(new MergeConfigurationTask(outFile, sourceFiles));
        }

        public static ITarget ReplaceText(this ITarget target, string sourceFile, params Tuple<string, string>[] tokens)
        {
            return target.AddTask(new ReplaceTextTask(sourceFile).Replace(tokens));
        }
    }
}
