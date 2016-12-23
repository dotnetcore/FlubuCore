using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FlubuCore.Context
{
    public static class ContextPropertiesExtensions
    {
        public static OSPlatform GetOSPlatform(this IBuildPropertiesSession context)
        {
            return context.TryGet<OSPlatform>(BuildProps.OSPlatform);
        }

        public static OSPlatform SetOSPlatform(this ITaskContextInternal context, OSPlatform version)
        {
            context.Properties.Set(BuildProps.OSPlatform, version);
            return version;
        }

        public static DefaultTargets GetDefaultTargets(this IBuildPropertiesSession context)
        {
            return context.TryGet<DefaultTargets>(BuildProps.DefaultTargets);
        }

        public static DefaultTargets SetDefaultTargets(this IBuildPropertiesSession context, DefaultTargets defaultTargets)
        {
            context.Set(BuildProps.DefaultTargets, defaultTargets);
            return defaultTargets;
        }

        public static string GetNodeExecutablePath(this IBuildPropertiesSession context)
        {
            return context.TryGet<string>(BuildProps.NodeExecutablePath);
        }

        public static string SetNodeExecutablePath(this ITaskContextInternal context, string path)
        {
            context.Properties.Set(BuildProps.NodeExecutablePath, path);
            return path;
        }

        public static string GetNpmPath(this IBuildPropertiesSession context)
        {
            return context.TryGet<string>(BuildProps.NpmPath);
        }

        public static string SetNpmPath(this ITaskContextInternal context, string path)
        {
            context.Properties.Set(BuildProps.NpmPath, path);
            return path;
        }

        public static string GetProfileFolder(this IBuildPropertiesSession context)
        {
            return context.TryGet<string>(BuildProps.UserHomeFolder);
        }

        public static string SetProfileFolder(this ITaskContextInternal context, string path)
        {
            context.Properties.Set(BuildProps.UserHomeFolder, path);
            return path;
        }

        public static Version GetBuildVersion(this IBuildPropertiesSession context)
        {
            return context.TryGet<Version>(BuildProps.BuildVersion);
        }

        public static Version SetBuildVersion(this ITaskContextInternal context, Version version)
        {
            context.Properties.Set(BuildProps.BuildVersion, version);
            return version;
        }

        public static string GetDotnetExecutable(this IBuildPropertiesSession context)
        {
            return context.TryGet<string>(BuildProps.DotNetExecutable);
        }

        public static string SetDotnetExecutable(this ITaskContextInternal context, string fullPath)
        {
            context.Properties.Set(BuildProps.DotNetExecutable, fullPath);
            return fullPath;
        }

        public static string SetBuildDir(this ITaskContextInternal context, string path)
        {
            context.Properties.Set(BuildProps.BuildDir, path);
            return path;
        }

        public static string GetBuildDir(this IBuildPropertiesSession context)
        {
            return context.TryGet<string>(BuildProps.BuildDir);
        }

        public static string SetOutputDir(this ITaskContextInternal context, string path)
        {
            context.Properties.Set(BuildProps.OutputDir, path);
            return path;
        }

        public static string GetOutputDir(this IBuildPropertiesSession context)
        {
            return context.TryGet<string>(BuildProps.OutputDir);
        }

        public static string SetProductRootDir(this ITaskContextInternal context, string path)
        {
            context.Properties.Set(BuildProps.ProductRootDir, path);
            return path;
        }

        public static string GetProductRootDir(this IBuildPropertiesSession context)
        {
            return context.TryGet<string>(BuildProps.ProductRootDir);
        }

        public static T TryGet<T>(this IBuildPropertiesSession context, string propName, T defaultValue = default(T))
        {
            T ret = context.TryGet<T>(propName);

            if (EqualityComparer<T>.Default.Equals(ret, default(T)))
                ret = defaultValue;

            return ret;
        }

        public static T Set<T>(this IBuildPropertiesSession context, string propName, T value)
        {
            context.Set(propName, value);
            return value;
        }
    }
}
