using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlubuCore.Context
{
    public interface IBuildPropertiesSession
    {
        /// <summary>
        /// Property indexer.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        string this[string propertyName]
        {
            get;
            set;
        }

        /// <summary>
        /// Get's the property by property name.
        /// </summary>
        /// <typeparam name="T">Type of returned property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <param name="memberName">Leave it empty! Property is auto assigned by FlubuCore.</param>
        /// <returns>The property.</returns>
        T Get<T>(string propertyName, bool ignoreCase = true, [CallerMemberName] string memberName = "");

        /// <summary>
        /// Get's the property that predefined by flubu by property name. All properties can also be overriden.
        /// </summary>
        /// <typeparam name="T">Type of returned property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The property.</returns>
        T Get<T>(PredefinedBuildProperties propertyName);

        /// <summary>
        /// Get's the property by property name. If it doesn't exist null is returned.
        /// </summary>
        /// <typeparam name="T">Type of returned property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <param name="memberName">Leave it empty! Property is auto assigned by FlubuCore.</param>
        /// <returns>The property.</returns>
        T TryGet<T>(string propertyName, [CallerMemberName] string memberName = "");

        /// <summary>
        /// Get's the property by property name.
        /// </summary>
        /// <typeparam name="T">Type of returned property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <param name="defaultValue">Returned value if property is not set in session.</param>
        /// <param name="memberName">Leave it empty! Property is auto assigned by FlubuCore.</param>
        /// <returns>The property.</returns>
        T Get<T>(string propertyName, T defaultValue, [CallerMemberName] string memberName = "");

        /// <summary>
        /// Checks by property name if property is stored in session.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        bool Has(string propertyName);

        /// <summary>
        /// Enumerates all properties.
        /// </summary>
        /// <returns>Enumareted properties.</returns>
        IEnumerable<KeyValuePair<string, object>> EnumerateProperties();

        /// <summary>
        /// Set's property in session.
        /// </summary>
        /// <typeparam name="T">Type of property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">The propery value.</param>
        /// <param name="injectToProperties">This should be only changed by Flubu internal logic</param>
        void Set<T>(string propertyName, T propertyValue, bool injectToProperties = true);

        /// <summary>
        /// Clear all properties from session.
        /// </summary>
        void Clear();

        /// <summary>
        /// Removes the specified property from session.
        /// </summary>
        /// <param name="propertyName">The name of property to be removed.</param>
        void Remove(string propertyName);
    }
}
