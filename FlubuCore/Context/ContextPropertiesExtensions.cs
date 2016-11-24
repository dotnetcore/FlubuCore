using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FlubuCore.Context
{
    public static class ContextPropertiesExtensions
    {
        public static OSPlatform GetOSPlatform(this ITaskContextInternal context)
        {
            return context.Properties.TryGet<OSPlatform>(BuildProps.OSPlatform);
        }

        public static OSPlatform SetOSPlatform(this ITaskContextInternal context, OSPlatform version)
        {
            context.Properties.Set<OSPlatform>(BuildProps.OSPlatform, version);
            return version;
        }

        public static string GetNodeExecutablePath(this ITaskContextInternal context)
        {
            return context.Properties.TryGet<string>(BuildProps.NodeExecutablePath);
        }

        public static string SetNodeExecutablePath(this ITaskContextInternal context, string path)
        {
            context.Properties.Set<string>(BuildProps.NodeExecutablePath, path);
            return path;
        }

        public static string GetNpmPath(this ITaskContextInternal context)
        {
            return context.Properties.TryGet<string>(BuildProps.NpmPath);
        }

        public static string SetNpmPath(this ITaskContextInternal context, string path)
        {
            context.Properties.Set<string>(BuildProps.NpmPath, path);
            return path;
        }

        public static string GetProfileFolder(this ITaskContextInternal context)
        {
            return context.Properties.TryGet<string>(BuildProps.UserHomeFolder);
        }

        public static string SetProfileFolder(this ITaskContextInternal context, string path)
        {
            context.Properties.Set<string>(BuildProps.UserHomeFolder, path);
            return path;
        }

        public static Version GetBuildVersion(this ITaskContextInternal context)
        {
            return context.Properties.TryGet<Version>(BuildProps.BuildVersion);
        }

        public static Version SetBuildVersion(this ITaskContextInternal context, Version version)
        {
            context.Properties.Set<Version>(BuildProps.BuildVersion, version);
            return version;
        }

        public static string GetDotnetExecutable(this ITaskContextInternal context)
        {
            return context.Properties.TryGet<string>(BuildProps.DotNetExecutable);
        }

        public static string SetDotnetExecutable(this ITaskContextInternal context, string fullPath)
        {
            context.Properties.Set<string>(BuildProps.DotNetExecutable, fullPath);
            return fullPath;
        }

        public static string SetBuildDir(this ITaskContextInternal context, string path)
        {
            context.Properties.Set<string>(BuildProps.BuildDir, path);
            return path;
        }

        public static string GetBuildDir(this ITaskContextInternal context)
        {
            return context.Properties.TryGet<string>(BuildProps.BuildDir);
        }

        public static string SetOutputDir(this ITaskContextInternal context, string path)
        {
            context.Properties.Set<string>(BuildProps.OutputDir, path);
            return path;
        }

        public static string GetOutputDir(this ITaskContextInternal context)
        {
            return context.Properties.TryGet<string>(BuildProps.OutputDir);
        }

        public static string SetProductRootDir(this ITaskContextInternal context, string path)
        {
            context.Properties.Set<string>(BuildProps.ProductRootDir, path);
            return path;
        }

        public static string GetProductRootDir(this ITaskContextInternal context)
        {
            return context.Properties.TryGet<string>(BuildProps.ProductRootDir);
        }

        public static T TryGet<T>(this ITaskContextInternal context, string propName, T defaultValue = default(T))
        {
            T ret = context.Properties.TryGet<T>(propName);

            if (EqualityComparer<T>.Default.Equals(ret, default(T)))
                ret = defaultValue;

            return ret;
        }

        public static T Set<T>(this ITaskContextInternal context, string propName, T value)
        {
            context.Properties.Set<T>(propName, value);
            return value;
        }
    }
}
