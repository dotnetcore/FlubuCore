using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FlubuCore.Context
{
    public static class ContextPropertiesExtensions
    {
        public static OSPlatform GetOSPlatform(this ITaskContext context)
        {
            return context.Properties.TryGet<OSPlatform>(BuildProps.OSPlatform);
        }

        public static OSPlatform SetOSPlatform(this ITaskContext context, OSPlatform version)
        {
            context.Properties.Set<OSPlatform>(BuildProps.OSPlatform, version);
            return version;
        }

        public static string GetNodeExecutablePath(this ITaskContext context)
        {
            return context.Properties.TryGet<string>(BuildProps.NodeExecutablePath);
        }

        public static string SetNodeExecutablePath(this ITaskContext context, string path)
        {
            context.Properties.Set<string>(BuildProps.NodeExecutablePath, path);
            return path;
        }

        public static string GetNpmPath(this ITaskContext context)
        {
            return context.Properties.TryGet<string>(BuildProps.NpmPath);
        }

        public static string SetNpmPath(this ITaskContext context, string path)
        {
            context.Properties.Set<string>(BuildProps.NpmPath, path);
            return path;
        }

        public static string GetProfileFolder(this ITaskContext context)
        {
            return context.Properties.TryGet<string>(BuildProps.UserHomeFolder);
        }

        public static string SetProfileFolder(this ITaskContext context, string path)
        {
            context.Properties.Set<string>(BuildProps.UserHomeFolder, path);
            return path;
        }

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
