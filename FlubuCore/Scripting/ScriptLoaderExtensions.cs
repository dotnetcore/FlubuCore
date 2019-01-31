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

        internal static AssemblyInfo ToAssemblyInfo(this Assembly assembly)
        {
            AssemblyInfo info = new AssemblyInfo();
            info.FullPath = assembly.Location;
            var assemblyName = assembly.GetName();
            info.Name = assemblyName.Name;
            info.Version = assemblyName.Version;
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
            var existedItem = assemblyInfos.FirstOrDefault(x => x.Name == item.Name);
            if (existedItem != null)
            {
                if (existedItem.VersionStatus == VersionStatus.Sealed)
                {
                    return;
                }

                if (item.Version == null)
                {
                    return;
                }

                var compare = existedItem.Version?.CompareTo(item.Version);
                if (compare == -1)
                {
                    existedItem.Version = item.Version;
                }
            }
            else
            {
                assemblyInfos.Add(item);
            }
        }
    }
}
