using System;
using System.Collections.Generic;

namespace FlubuCore.Context
{
    public static class ContextPropertiesExtensions
    {
        public static Version GetBuildVersion(this ITaskContext context)
        {
            return context.Properties.TryGet<Version>(BuildProps.BuildVersion);
        }

        public static Version SetBuildVersion(this ITaskContext context, Version version)
        {
            context.Properties.Set<Version>(BuildProps.BuildVersion, version);
            return version;
        }

        public static string GetDotnetExecutable(this ITaskContext context)
        {
            return context.Properties.TryGet<string>(BuildProps.DotNetExecutable);
        }

        public static string SetDotnetExecutable(this ITaskContext context, string fullPath)
        {
            context.Properties.Set<string>(BuildProps.DotNetExecutable, fullPath);
            return fullPath;
        }

        public static T TryGet<T>(this ITaskContext context, string propName, T defaultValue = default(T))
        {
            T ret = context.Properties.TryGet<T>(propName);

            if (EqualityComparer<T>.Default.Equals(ret, default(T)))
                ret = defaultValue;

            return ret;
        }

        public static T Set<T>(this ITaskContext context, string propName, T value)
        {
            context.Properties.Set<T>(propName, value);
            return value;
        }
    }
}
