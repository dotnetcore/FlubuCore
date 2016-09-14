using flubu.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace flubu
{
    public class SimpleTaskContextProperties : ITaskContextProperties
    {
        public T Get<T>(string propertyName)
        {
            if (!properties.ContainsKey(propertyName))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture, 
                    "Task context property '{0}' is missing.", 
                    propertyName);
                throw new KeyNotFoundException(message);
            }

            return (T)Convert.ChangeType(properties[propertyName], typeof(T), CultureInfo.InvariantCulture);
        }

        public T Get<T>(string propertyName, T defaultValue)
        {
            if (!properties.ContainsKey(propertyName))
                return defaultValue;
            return (T)Convert.ChangeType(properties[propertyName], typeof(T), CultureInfo.InvariantCulture);
        }

        public bool Has(string propertyName)
        {
            return properties.ContainsKey(propertyName);
        }

        public IEnumerable<KeyValuePair<string, object>> EnumerateProperties()
        {
            return properties;
        }

        public void Set<T>(string propertyName, T propertyValue)
        {
            properties[propertyName] = propertyValue;
        }

        public string this[string propertyName]
        {
            get { return Get<string>(propertyName); }
            set { Set(propertyName, value); }
        }

        public void Clear()
        {
            properties.Clear();
        }

        private readonly Dictionary<string, object> properties = new Dictionary<string, object>();
    }
}