using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using FlubuCore.Context.Attributes;
using FlubuCore.Targeting;

namespace FlubuCore.Context
{
    /// <summary>
    /// Used for storing task context properties into session.
    /// </summary>
    public class BuildPropertiesSession : IBuildPropertiesSession
    {
        private static Dictionary<string, PropertyInfo> _propertyInfos = null;

        /// <summary>
        /// name value dictionary used for storing differend task context properties.
        /// </summary>
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        private readonly TargetTree _targetTree;

        public BuildPropertiesSession(TargetTree targetTree)
        {
            _targetTree = targetTree;
        }

        /// <summary>
        /// Property indexer.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string this[string propertyName]
        {
            get { return Get<string>(propertyName); }
            set { Set(propertyName, value); }
        }

        /// <summary>
        /// Get's the property by property name.
        /// </summary>
        /// <typeparam name="T">Type of returned property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <param name="ignoreCase">if true key is not case sensitive. Otherwise it is.</param>
        /// <param name="memberName"></param>
        /// <returns>The property.</returns>
        public T Get<T>(string propertyName, bool ignoreCase = true, [CallerMemberName] string memberName = "")
        {
            InitializePropertyInfos();

            if (ignoreCase)
            {
                propertyName = propertyName.ToLowerInvariant();
            }

            if (_propertyInfos.ContainsKey(propertyName))
            {
                return GetValueFromPropertyInfo<T>(propertyName);
            }

            if (!_properties.ContainsKey(propertyName))
            {
                if (memberName == "ConfigureTargets")
                {
                    throw new KeyNotFoundException($"Task context property '{propertyName}' is missing. ConfigureTarget method is executed before all 'Do' and tasks are executed.");
                }

                throw new KeyNotFoundException($"Task context property '{propertyName}' is missing.");
            }

            return (T)Convert.ChangeType(_properties[propertyName], typeof(T), CultureInfo.InvariantCulture);
        }

        public T Get<T>(PredefinedBuildProperties propertyName)
        {
            string propName;

            switch (propertyName)
            {
                case PredefinedBuildProperties.OsPlatform:
                    propName = BuildProps.OSPlatform;
                    break;
                case PredefinedBuildProperties.OutputDir:
                    propName = BuildProps.OutputDir;
                    break;
                case PredefinedBuildProperties.PathToDotnetExecutable:
                    propName = BuildProps.DotNetExecutable;
                    break;
                case PredefinedBuildProperties.ProductRootDir:
                    propName = BuildProps.ProductRootDir;
                    break;
                case PredefinedBuildProperties.UserProfileFolder:
                    propName = BuildProps.UserHomeFolder;
                    break;
                case PredefinedBuildProperties.IsWebApi:
                    propName = BuildProps.IsWebApi;
                    break;
                default:
                    throw new NotSupportedException("Property name is not mapped.");
            }

            propName = propName.ToLowerInvariant();

            return Get<T>(propName);
        }

        public T TryGet<T>(string propertyName, [CallerMemberName] string memberName = null)
        {
            InitializePropertyInfos();

            propertyName = propertyName.ToLowerInvariant();

            if (_propertyInfos.ContainsKey(propertyName))
            {
                return GetValueFromPropertyInfo<T>(propertyName);
            }

            if (!_properties.ContainsKey(propertyName))
            {
                if (memberName == "ConfigureTargets")
                {
                    throw new KeyNotFoundException($"Task context property '{propertyName}' is missing. ConfigureTarget method is executed before all 'Do' and tasks are executed.");
                }

                return default(T);
            }

            return (T)Convert.ChangeType(_properties[propertyName], typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get's the property by property name.
        /// </summary>
        /// <typeparam name="T">Type of returned property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <param name="defaultValue">Returned value if property is not set in session.</param>
        /// <returns>The property.</returns>
        public T Get<T>(string propertyName, T defaultValue, [CallerMemberName] string memberName = "")
        {
            propertyName = propertyName.ToLowerInvariant();
            InitializePropertyInfos();
            if (_propertyInfos.ContainsKey(propertyName))
            {
                return GetValueFromPropertyInfo<T>(propertyName);
            }

            if (!_properties.ContainsKey(propertyName))
            {
                if (memberName.Equals("ConfigureTargets"))
                {
                    throw new KeyNotFoundException($"Task context property '{propertyName}' is missing. ConfigureTarget method is executed before all 'Do' and tasks are executed.");
                }

                return defaultValue;
            }

            return (T)Convert.ChangeType(_properties[propertyName], typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Checks by property name if property is stored in session.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool Has(string propertyName)
        {
            propertyName = propertyName.ToLowerInvariant();
            return _properties.ContainsKey(propertyName);
        }

        /// <summary>
        /// Enumerates all properties.
        /// </summary>
        /// <returns>Enumareted properties.</returns>
        public IEnumerable<KeyValuePair<string, object>> EnumerateProperties()
        {
            return _properties;
        }

        /// <summary>
        /// Set's property in session.
        /// </summary>
        /// <typeparam name="T">Type of property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">The propery value.</param>
        public void Set<T>(string propertyName, T propertyValue)
        {
            InitializePropertyInfos();
            propertyName = propertyName.ToLowerInvariant();

            if (_propertyInfos.ContainsKey(propertyName) && _targetTree.BuildScript != null)
            {
                var propertyInfo = _propertyInfos[propertyName];
                propertyInfo.SetValue(_targetTree.BuildScript, propertyValue);
            }

            _properties[propertyName] = propertyValue;
        }

        /// <summary>
        /// Clear all properties from session.
        /// </summary>
        public void Clear()
        {
            _properties.Clear();
        }

        /// <summary>
        /// Removes the specified property from session.
        /// </summary>
        /// <param name="propertyName">The name of property to be removed.</param>
        public void Remove(string propertyName)
        {
            propertyName = propertyName.ToLowerInvariant();
            _properties.Remove(propertyName);
        }

        private void InitializePropertyInfos()
        {
            if (_propertyInfos != null)
            {
                return;
            }

            _propertyInfos = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

            if (_targetTree.BuildScript != null)
            {
                var buildScriptType = _targetTree.BuildScript.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(buildScriptType.GetProperties());

                foreach (var propertyInfo in props)
                {
                    var attr = propertyInfo.GetCustomAttribute<BuildPropertyAttribute>();
                    if (attr != null)
                    {
                        _propertyInfos.Add(attr.BuildProperty, propertyInfo);
                    }
                }
            }
        }

        private T GetValueFromPropertyInfo<T>(string propertyName)
        {
            var propertyInfo = _propertyInfos[propertyName];
            var value = propertyInfo.GetValue(_targetTree.BuildScript);
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}
