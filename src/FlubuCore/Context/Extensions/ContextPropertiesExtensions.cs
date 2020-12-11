using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FlubuCore.IO;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;
using FlubuCore.Tasks.Versioning;
using GlobExpressions;

namespace FlubuCore.Context
{
    public static class ContextPropertiesExtensions
    {
        public static VSSolution GetVsSolution(this IBuildPropertiesSession context)
        {
            return context.TryGet<VSSolution>(BuildProps.Solution);
        }

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
            return context.TryGet<DefaultTargets>(DotNetBuildProps.DefaultTargets);
        }

        public static DefaultTargets SetDefaultTargets(this IBuildPropertiesSession context,
            DefaultTargets defaultTargets)
        {
            context.Set(DotNetBuildProps.DefaultTargets, defaultTargets);
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
            return context.TryGet<string>(BuildProps.UserProfileFolder);
        }

        public static string SetProfileFolder(this ITaskContextInternal context, string path)
        {
            context.Properties.Set(BuildProps.UserProfileFolder, path);
            return path;
        }

        public static BuildVersion GetBuildVersion(this IBuildPropertiesSession context)
        {
            return context.Get<BuildVersion>(BuildProps.BuildVersion, null);
        }

        public static BuildVersion SetBuildVersion(this IBuildPropertiesSession context, BuildVersion version)
        {
            context.Set(BuildProps.BuildVersion, version);
            return version;
        }

        public static BuildVersion SetBuildVersion(this IBuildPropertiesSession context, int major, int minor, int build, int revision)
        {
            return context.SetBuildVersion(new BuildVersion(new Version(major, minor, build, revision)));
        }

        public static BuildVersion SetBuildVersion(this ITaskContext context, BuildVersion version)
        {
            context.Properties.SetBuildVersion(version);
            return version;
        }

        public static BuildVersion SetBuildVersion(this ITaskContext context, int major, int minor, int build, int revision)
        {
            return context.Properties.SetBuildVersion(major, minor, build, revision);
        }

        public static string GetFlubuWebApiBaseUrl(this IBuildPropertiesSession context)
        {
            return context.TryGet<string>(BuildProps.FlubuWebApiBaseUrl);
        }

        public static string SetFlubuWebApiBaseUrl(this IBuildPropertiesSession context, string webApiUrl)
        {
            context.Set(BuildProps.FlubuWebApiBaseUrl, webApiUrl);
            return webApiUrl;
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
            context.Properties.Set(DotNetBuildProps.BuildDir, path);
            return path;
        }

        public static string GetBuildDir(this IBuildPropertiesSession context)
        {
            return context.TryGet<string>(DotNetBuildProps.BuildDir);
        }

        public static string SetOutputDir(this ITaskContextInternal context, string path)
        {
            context.Properties.Set(DotNetBuildProps.OutputDir, path);
            return path;
        }

        public static string GetOutputDir(this IBuildPropertiesSession context)
        {
            return context.TryGet<string>(DotNetBuildProps.OutputDir);
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
            var ret = context.TryGet<T>(propName);

            if (EqualityComparer<T>.Default.Equals(ret, default(T)))
                ret = defaultValue;

            return ret;
        }

        public static T Set<T>(this IBuildPropertiesSession context, string propName, T value)
        {
            context.Set(propName, value);
            return value;
        }

        /// <summary>
        ///     Get the context variable for SqlCmd executable.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetSqlCmdExecutable(this IBuildPropertiesSession context)
        {
            return context.TryGet<string>(BuildProps.SqlCmdExecutable);
        }

        /// <summary>
        ///     Set the context variable for SqlCmd executable.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string SetSqlCmdExecutable(this ITaskContextInternal context, string fullPath)
        {
            context.Properties.Set(BuildProps.SqlCmdExecutable, fullPath);
            return fullPath;
        }

        /// <summary>
        /// Gets Visual studio solution information. if <see cref="solutionFileName"/> is not specified solution file name is readed from <see cref="IBuildPropertiesContext"/> property <see cref="BuildProps.SolutionFileName"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="solutionFileName"></param>
        /// <returns></returns>
        public static VSSolution GetVsSolution(this IBuildPropertiesContext context, string solutionFileName = null)
        {
            var taskContext = context as ITaskContext;
            if (taskContext == null)
            {
                return null;
            }

            return ContextBaseExtensions.GetVsSolution(taskContext, solutionFileName);
        }

        /// <summary>
        /// Gets all files matching glob pattern.
        /// See: https://github.com/kthompson/glob for supported pattern expressions and use cases.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public static List<FileFullPath> GetFiles(this IBuildPropertiesContext context, string directory,  params string[] globPattern)
        {
            return ContextBaseExtensions.GetFiles(directory, globPattern);
        }

        /// <summary>
        /// Gets all files matching glob pattern.
        /// See: https://github.com/kthompson/glob for supported pattern expressions and use cases.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public static List<FileFullPath> GetFiles(this IBuildPropertiesContext context, string directory, GlobOptions globOptions = GlobOptions.None, params string[] globPattern)
        {
           return ContextBaseExtensions.GetFiles(directory, globOptions, globPattern);
        }

        /// <summary>
        /// Gets all directories matching glob pattern.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public static List<FullPath> GetDirectories(this IBuildPropertiesContext context, string directory, params string[] globPattern)
        {
           return ContextBaseExtensions.GetDirectories(directory, GlobOptions.None, globPattern);
        }

        /// <summary>
        /// Gets all directories matching glob pattern.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public static List<FullPath> GetDirectories(this IBuildPropertiesContext context, string directory, GlobOptions globOptions = GlobOptions.None, params string[] globPattern)
        {
            return ContextBaseExtensions.GetDirectories(directory, globOptions, globPattern);
        }

        public static FullPath GetRootDirectory(this IBuildPropertiesContext context)
        {
            return new FullPath(context.Properties.GetProductRootDir());
        }
    }
}