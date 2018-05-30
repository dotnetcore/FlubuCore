using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FlubuCore.Scripting;

namespace FlubuCore.Context
{
    public static class ScriptPropertiesSetter
    {
        public static void SetPropertiesFromArg(IBuildScript buildScript, ITaskSession taskSession)
        {
            var buildScriptType = buildScript.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(buildScriptType.GetProperties());

            foreach (var property in props)
            {
                var attributes = property.GetCustomAttributes<FromArgAttribute>(false).ToList();
                foreach (var fromArgAttribute in attributes)
                {
                    if (!taskSession.ScriptArgs.ContainsKey(fromArgAttribute.ArgKey))
                    {
                        continue;
                    }

                    property.SetValue(buildScript, MethodParameterModifier.ParseValueByType(taskSession.ScriptArgs[fromArgAttribute.ArgKey], property.PropertyType));
                }

                if (taskSession.ScriptArgs.ContainsKey(property.Name))
                {
                    property.SetValue(buildScript,  MethodParameterModifier.ParseValueByType(taskSession.ScriptArgs[property.Name], property.PropertyType));
                }
            }
        }
    }
}
