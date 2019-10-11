using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FlubuCore.Infrastructure.Terminal;
using FlubuCore.Scripting;

namespace FlubuCore.Context
{
    public static class ScriptProperties
    {
        public static void SetPropertiesFromScriptArg(IBuildScript buildScript, IFlubuSession flubuSession)
        {
            var buildScriptType = buildScript.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(buildScriptType.GetProperties());

            foreach (var property in props)
            {
                var attributes = property.GetCustomAttributes<FromArgAttribute>(false).ToList();
                string argKey = null;
                foreach (var fromArgAttribute in attributes)
                {
                    var argKeys = fromArgAttribute.ArgKey.Split('|');
                    foreach (var key in argKeys)
                    {
                        if (!flubuSession.ScriptArgs.ContainsKey(key))
                        {
                            continue;
                        }

                        argKey = key;
                        break;
                    }

                    if (argKey == null)
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
                            var list = flubuSession.ScriptArgs[argKey].Split(fromArgAttribute.Seperator)
                                .ToList();
                            property.SetValue(buildScript, list);
                        }
                    }
                    else
                    {
                        SetPropertyValue(property, buildScript, flubuSession.ScriptArgs[argKey], property.PropertyType, argKey);
                    }
                }

                if (flubuSession.ScriptArgs.ContainsKey(property.Name))
                {
                    if (property.PropertyType.GetTypeInfo().IsGenericType)
                    {
                        var propertyGenericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                        if (propertyGenericTypeDefinition == typeof(IList<>) ||
                            propertyGenericTypeDefinition == typeof(List<>) ||
                            propertyGenericTypeDefinition == typeof(IEnumerable<>))
                        {
                            property.SetValue(buildScript, flubuSession.ScriptArgs[property.Name].Split(',').ToList());
                        }
                    }
                    else
                    {
                        SetPropertyValue(property, buildScript, flubuSession.ScriptArgs[property.Name], property.PropertyType, property.Name);
                    }
                }
            }
        }

        private static void SetPropertyValue(PropertyInfo propertyInfo, IBuildScript buildScript, string value, Type type, string argKey)
        {
            try
            {
                propertyInfo.SetValue(buildScript, MethodParameterModifier.ParseValueByType(value, type));
            }
            catch (FormatException e)
            {
                throw new ScriptException($"Could not pass value '{value}' from argument '{argKey}' to build script property '{propertyInfo.Name}'", e);
            }
            catch (ArgumentException e)
            {
                throw new ScriptException($"Could not pass value '{value}' from argument '{argKey}' to build script property '{propertyInfo.Name}'", e);
            }
        }

        public static List<Hint> GetPropertiesKeys(IBuildScript buildScript, IFlubuSession flubuSession)
        {
            var buildScriptType = buildScript.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(buildScriptType.GetProperties());
            List<Hint> keys = new List<Hint>();
            foreach (var property in props)
            {
                var attributes = property.GetCustomAttributes<FromArgAttribute>(false).ToList();
                if (attributes.Count == 0)
                {
                    keys.Add(new Hint { Name = property.Name });
                }
                else
                {
                    foreach (var fromArgAttribute in attributes)
                    {
                        keys.Add(new Hint
                        {
                            Name = fromArgAttribute.ArgKey,
                            Help = fromArgAttribute.Help,
                            HintColor = ConsoleColor.DarkCyan
                        });
                    }
                }
            }

            return keys;
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
                        help.Add($"-{fromArgAttribute.ArgKey.Replace("|", "|-")} : {fromArgAttribute.Help}");
                    }
                }
            }

            return help;
        }
    }
}
