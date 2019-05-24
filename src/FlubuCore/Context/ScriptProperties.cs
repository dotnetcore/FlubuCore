using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FlubuCore.Scripting;

namespace FlubuCore.Context
{
    public static class ScriptProperties
    {
        public static void SetPropertiesFromScriptArg(IBuildScript buildScript, ITaskSession taskSession)
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

                    if (property.PropertyType.GetTypeInfo().IsGenericType)
                    {
                        var propertyGenericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                        if (propertyGenericTypeDefinition == typeof(IList<>) ||
                            propertyGenericTypeDefinition == typeof(List<>) ||
                            propertyGenericTypeDefinition == typeof(IEnumerable<>))
                        {
                            var list = taskSession.ScriptArgs[fromArgAttribute.ArgKey].Split(fromArgAttribute.Seperator)
                                .ToList();
                            property.SetValue(buildScript, list);
                        }
                    }
                    else
                    {
                        property.SetValue(buildScript, MethodParameterModifier.ParseValueByType(taskSession.ScriptArgs[fromArgAttribute.ArgKey], property.PropertyType));
                    }
                }

                if (taskSession.ScriptArgs.ContainsKey(property.Name))
                {
                    if (property.PropertyType.GetTypeInfo().IsGenericType)
                    {
                        var propertyGenericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                        if (propertyGenericTypeDefinition == typeof(IList<>) ||
                            propertyGenericTypeDefinition == typeof(List<>) ||
                            propertyGenericTypeDefinition == typeof(IEnumerable<>))
                        {
                            property.SetValue(buildScript, taskSession.ScriptArgs[property.Name].Split(',').ToList());
                        }
                    }
                    else
                    {
                        property.SetValue(buildScript, MethodParameterModifier.ParseValueByType(taskSession.ScriptArgs[property.Name], property.PropertyType));
                    }
                }
            }
        }

        public static List<string> GetPropertiesHelp(IBuildScript buildScript)
        {
            var buildScriptType = buildScript.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(buildScriptType.GetProperties());
            List<string> help = new List<string>();
            foreach (var property in props)
            {
                var attributes = property.GetCustomAttributes<FromArgAttribute>(false).ToList();
                foreach (var fromArgAttribute in attributes)
                {
                    if (!string.IsNullOrEmpty(fromArgAttribute.Help))
                    {
                        help.Add($"-{fromArgAttribute.ArgKey} : {fromArgAttribute.Help}");
                    }
                }
            }

            return help;
        }
    }
}
