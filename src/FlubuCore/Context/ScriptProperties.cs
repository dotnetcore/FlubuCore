using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FlubuCore.Infrastructure.Terminal;
using FlubuCore.Scripting;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Context
{
    public class ScriptProperties : IScriptProperties
    {
        public void InjectProperties(IBuildScript buildScript, IFlubuSession flubuSession)
        {
            var buildScriptType = buildScript.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(buildScriptType.GetProperties());

            foreach (var property in props)
            {
                InjectPropertiesFromScriptArg(buildScript, flubuSession, property);
                InjectPropertiesFromTaskAttributes(buildScript, flubuSession, property);
            }
        }

        public List<Hint> GetPropertiesHints(IBuildScript buildScript)
        {
            var buildScriptType = buildScript.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(buildScriptType.GetProperties());
            List<Hint> hints = new List<Hint>();
            foreach (var property in props)
            {
                var attributes = property.GetCustomAttributes<FromArgAttribute>(false).ToList();
                if (attributes.Count == 0)
                {
                    hints.Add(new Hint { Name = $"-{property.Name}" });
                }
                else
                {
                    foreach (var fromArgAttribute in attributes)
                    {
                        var keys = fromArgAttribute.ArgKey.Split('|');
                        hints.Add(new Hint
                        {
                            Name = $"-{keys[0]}",
                            Help = fromArgAttribute.Help,
                            HintColor = ConsoleColor.DarkCyan
                        });
                    }
                }
            }

            return hints;
        }

        public Dictionary<string, IReadOnlyCollection<Hint>> GetEnumHints(IBuildScript buildScript, IFlubuSession flubuSession)
        {
            var buildScriptType = buildScript.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(buildScriptType.GetProperties());
            Dictionary<string, IReadOnlyCollection<Hint>> hints = new Dictionary<string, IReadOnlyCollection<Hint>>();
            foreach (var property in props)
            {
                var type = property.PropertyType;
                if (type.IsEnum)
                {
                    var attribute = property.GetCustomAttribute<FromArgAttribute>(false);
                    string argKey = null;

                    if (attribute != null)
                    {
                        argKey = attribute.ArgKey.Split('|')[0];
                    }
                    else
                    {
                        argKey = property.Name;
                    }

                    var enumValues = Enum.GetValues(type);

                    List<Hint> values = new List<Hint>();
                    foreach (var enumValue in enumValues)
                    {
                        values.Add(new Hint
                        {
                            Name = enumValue.ToString(),
                            HintType = HintType.Value,
                        });
                    }

                    hints.Add(argKey.ToLower(), values);
                }
            }

            return hints;
        }

        public List<string> GetPropertiesHelp(IBuildScript buildScript)
        {
            var buildScriptType = buildScript.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(buildScriptType.GetProperties());
            List<string> help = new List<string>();
            foreach (var property in props)
            {
                var attributes = property.GetCustomAttributes<FromArgAttribute>(false).ToList();
                foreach (var fromArgAttribute in attributes)
                {
                    var key = $"-{fromArgAttribute.ArgKey.Replace("|", "|-")}";
                    var type = property.PropertyType;
                    if (type.IsEnum)
                    {
                        var enumValues = Enum.GetValues(type);

                        key = $"{key}(";
                        foreach (var enumValue in enumValues)
                        {
                            key = $"{key}{enumValue}, ";
                        }

                        key = $"{key.Substring(0, key.Length - 2)})";
                    }

                    if (!string.IsNullOrEmpty(fromArgAttribute.Help))
                    {
                        help.Add($"{key} : {fromArgAttribute.Help}");
                    }
                }
            }

            return help;
        }

        private static void InjectPropertiesFromTaskAttributes(IBuildScript buildScript, IFlubuSession flubuSession,  PropertyInfo property)
        {
            InjectPropertyFromFetchBuildVersionFomFileAttribute(flubuSession, buildScript, property);
            InjectPropertyFromGitVersionAttribute(flubuSession, buildScript, property);
            InjectPropertyFromLoadSolutionAttribute(flubuSession, buildScript, property);
        }

        private static void InjectPropertiesFromScriptArg(IBuildScript buildScript, IFlubuSession flubuSession,  PropertyInfo property)
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
                    SetPropertyValue(property, buildScript, flubuSession.ScriptArgs[property.Name], property.PropertyType,
                        property.Name);
                }
            }
        }

        private static void InjectPropertyFromFetchBuildVersionFomFileAttribute(IFlubuSession flubuSession, IBuildScript buildScript, PropertyInfo property)
        {
            var fetchBuildVersion = property.GetCustomAttribute<FetchBuildVersionFromFileAttribute>();

            if (fetchBuildVersion == null)
            {
                return;
            }

            var task = flubuSession.Tasks().FetchBuildVersionFromFileTask().WithLogLevel(LogLevel.None)
                .When(() => fetchBuildVersion.AllowSuffix, t => t.AllowSuffix())
                .When(() => !string.IsNullOrEmpty(fetchBuildVersion.ProjectVersionFileName),
                    t => t.ProjectVersionFileName(fetchBuildVersion.ProjectVersionFileName))
                .When(() => fetchBuildVersion.PrefixesToRemove != null, t =>
                {
                    foreach (var prefixToRemove in fetchBuildVersion.PrefixesToRemove)
                    {
                        t.RemovePrefix(prefixToRemove);
                    }
                });

            task.LogTaskExecutionInfo = false;

            var buildVersion = task.Execute(flubuSession);

            if (property.PropertyType != typeof(BuildVersion))
            {
                throw new ScriptException($"Failed to fetch build version. Property '{property.Name}' must be of type '{nameof(BuildVersion)}'");
            }

            property.SetValue(buildScript, buildVersion);
        }

        private static void InjectPropertyFromGitVersionAttribute(IFlubuSession flubuSession, IBuildScript buildScript, PropertyInfo property)
        {
            var gitVersionAttr = property.GetCustomAttribute<GitVersionAttribute>();

            if (gitVersionAttr == null)
            {
                return;
            }

            var task = flubuSession.Tasks().GitVersionTask().WithLogLevel(LogLevel.None);
            task.LogTaskExecutionInfo = false;

            var gitVersion = task.Execute(flubuSession);

            if (property.PropertyType != typeof(GitVersion))
            {
                throw new ScriptException($"Failed to fetch git version. Property '{property.Name}' must be of type '{nameof(GitVersion)}'");
            }

            property.SetValue(buildScript, gitVersion);
        }

        private static void InjectPropertyFromLoadSolutionAttribute(IFlubuSession flubuSession, IBuildScript buildScript, PropertyInfo property)
        {
            var loadSolutionAttribute = property.GetCustomAttribute<LoadSolutionAttribute>();

            if (loadSolutionAttribute == null)
            {
                return;
            }

            if (property.PropertyType != typeof(VSSolution))
            {
                throw new ScriptException($"Failed to fetch Solution information. Property '{property.Name}' must be of type '{nameof(VSSolution)}'");
            }

            var vsSolution = !string.IsNullOrEmpty(loadSolutionAttribute.SolutionName)
                ? flubuSession.Tasks().LoadSolutionTask(loadSolutionAttribute.SolutionName)
                    .Execute(flubuSession)
                : flubuSession.Tasks().LoadSolutionTask()
                    .Execute(flubuSession);

            property.SetValue(buildScript, vsSolution);
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
    }
}
