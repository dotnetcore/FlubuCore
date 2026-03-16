using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FlubuCore.Scripting
{
    public static class ScriptLoaderExtensions
    {
        internal static void AddReferenceByAssemblyName(this List<AssemblyInfo> references, string assemblyName)
        {
            try
            {
                var assName = new AssemblyName(assemblyName);
                var assembly = Assembly.Load(assName);
                if (assembly == null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(assembly.Location))
                {
                    references.AddOrUpdateAssemblyInfo(new AssemblyInfo
                    {
                        Name = assName.Name,
                        Version = assName.Version,
                        FullPath = assembly.Location
                    });
                }
            }
            catch
            {
            }
        }

        internal static AssemblyInfo ToAssemblyInfo(this Assembly assembly, VersionStatus? versionStatus = null)
        {
            AssemblyInfo info = new AssemblyInfo();
            info.FullPath = assembly.Location;
            var assemblyName = assembly.GetName();
            info.Name = assemblyName.Name;
            info.Version = assemblyName.Version;

            if (versionStatus.HasValue)
            {
                info.VersionStatus = versionStatus.Value;
            }

            return info;
        }

        internal static void AddOrUpdateAssemblyInfo(this List<AssemblyInfo> assemblyInfos, List<AssemblyInfo> items)
        {
            foreach (var item in items)
            {
                AddOrUpdateAssemblyInfo(assemblyInfos, item);
            }
        }

        internal static void AddOrUpdateAssemblyInfo(this List<AssemblyInfo> assemblyInfos, AssemblyInfo item)
        {
            var existedItem = assemblyInfos.FirstOrDefault(
                x => string.Equals(x.Name, item.Name, StringComparison.OrdinalIgnoreCase));

            if (existedItem == null)
            {
                assemblyInfos.Add(item);
                return;
            }

            if (existedItem.VersionStatus == VersionStatus.Sealed)
            {
                return;
            }

            bool shouldReplace = false;

            if (item.Version != null &&
                (existedItem.Version == null || existedItem.Version.CompareTo(item.Version) < 0))
            {
                shouldReplace = true;
            }
            else if (item.Version == null && !string.IsNullOrEmpty(item.FullPath))
            {
                // Explicit path-based references (e.g. csproj HintPath, script refs)
                // can replace existing entries even without a version.
                shouldReplace = true;
            }

            if (!shouldReplace)
            {
                return;
            }

            existedItem.Version = item.Version;
            existedItem.FullPath = item.FullPath;
            existedItem.RuntimePath = item.RuntimePath;
            existedItem.IsCompileOnly = item.IsCompileOnly;
            existedItem.IsFromDependencyContext = item.IsFromDependencyContext;
            existedItem.VersionStatus = item.VersionStatus;
        }
    }
}
