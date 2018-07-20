using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FlubuCore.Scripting
{
    public static class ScriptLoaderExtensions
    {
        internal static void AddReferenceByAssemblyName(this List<string> references, string assemblyName)
        {
            try
            {
                var assembly = Assembly.Load(new AssemblyName(assemblyName));
                if (assembly == null)
                {
                    return;
                }

                references.Add(assembly.Location);
            }
            catch
            {
            }
        }
    }
}
