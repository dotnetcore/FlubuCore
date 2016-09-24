using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Flubu.Tasks
{
    /// <summary>
    /// Used for storing task context properties into session.
    /// </summary>
    public class TaskContextProperties : ITaskContextProperties
    {
        /// <summary>
        /// name value dictionary used for storing differend task context properties.
        /// </summary>
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

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
        /// <typeparam name="T">Type of returned property</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <returns>The property</returns>
        public T Get<T>(string propertyName)
        {
            if (!_properties.ContainsKey(propertyName))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Task context property '{0}' is missing.",
                    propertyName);
                throw new KeyNotFoundException(message);
            }

            return (T)Convert.ChangeType(_properties[propertyName], typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get's the property by property name.
        /// </summary>
        /// <typeparam name="T">Type of returned property</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <param name="defaultValue">Returned value if property is not set in session.</param>
        /// <returns>The property</returns>
        public T Get<T>(string propertyName, T defaultValue)
        {
            if (!_properties.ContainsKey(propertyName))
            {
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
            return _properties.ContainsKey(propertyName);
        }

        /// <summary>
        /// Enumerates all properties
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
            _properties.Remove(propertyName);
        }
    }
}
