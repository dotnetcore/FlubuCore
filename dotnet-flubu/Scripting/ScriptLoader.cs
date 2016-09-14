using flubu.Scripting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace flubu.Console
{
    public interface IScriptLoader
    {
        IBuildScript FindAndCreateBuildScriptInstance(string fileName);
    }

    public class ScriptLoader : IScriptLoader
    {
        public IBuildScript FindAndCreateBuildScriptInstance(string fileName)
        {
            //todo implement with roslyn
            throw new NotImplementedException();

            //CSScript.AssemblyResolvingEnabled = true;
            //Assembly assembly = CSScript.Load(fileName);

            //Type myType = typeof(IBuildScript);
            //List<Type> classes =
            //    assembly.GetTypes().Where(i => myType.IsAssignableFrom(i)).ToList();
            //if (classes.Count <= 0)
            //{
            //    string message = string.Format(
            //        CultureInfo.InvariantCulture,
            //        "Used build script file '{0}' but it does not contain any IBuildScript implementation.",
            //        fileName);

            //    throw new BuildScriptLocatorException(message);
            //}

            //object scriptInstance = assembly.CreateInstance(classes[0].FullName);
            //return scriptInstance.AlignToInterface<IBuildScript>();
        }
    }
}