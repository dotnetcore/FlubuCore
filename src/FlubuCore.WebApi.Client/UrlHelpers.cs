using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FlubuCore.WebApi.Client
{
    public static class UrlHelpers
    {
        private static readonly Regex ParameterRegex = new Regex(@"{(.*?)}");

        public static string ReplaceParameterTemplatesInRelativePathWithValues(string path, object requestObj)
        {
            var parameterizedParts = path.Split('/', '?')
            .SelectMany(x => ParameterRegex.Matches(x).Cast<Match>())
            .ToList();

            foreach (var parametrizedPart in parameterizedParts)
            {
                var propertyName = parametrizedPart.Value.Substring(1, parametrizedPart.Length - 2);
                var property = requestObj.GetType().GetRuntimeProperty(propertyName);
                if (property == null)
                {
                    throw new ArgumentException(string.Format("Url has paramater {0} but no property in request matches it.", parametrizedPart.Value));
                }

                var value = property.GetValue(requestObj, null);

                if (value == null)
                {
                    throw new ArgumentNullException(propertyName);
                }

                path = path.Replace(parametrizedPart.Value, value.ToString());
            }

            return path;
        }

        public static string ToQueryString(this object request, string separator = ",")
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            IList<PropertyInfo> propertyInfos = request.GetType().GetRuntimeProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(request, null) != null)
                .ToList();
            //// Get all properties (incl. string), exept IEnumerable ones
            var properties = propertyInfos
                .Where(x => (x.GetValue(request, null) is string) || !(x.GetValue(request, null) is IEnumerable))
                .Select(x => new KeyValuePair<string, string>(x.Name, (x.GetValue(request, null) is DateTime) ? ((DateTime)x.GetValue(request, null)).ToString("yyyy-MM-ddTHH:mm:ss") : x.GetValue(request, null).ToString()))
                .ToList();

            // Get all IEnumerable properties (excl. string)
            var enumerableProperties = propertyInfos
                .Where(x => !(x.GetValue(request, null) is string) && x.GetValue(request, null) is IEnumerable)
                .ToList();

            // add all IEnumerable properties
            foreach (var prop in enumerableProperties)
            {
                foreach (var item in (IEnumerable)prop.GetValue(request, null))
                {
                    properties.Add(new KeyValuePair<string, string>(prop.Name, item.ToString()));
                }
            }

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join("&",
                properties.Select(x =>
                    string.Concat(Uri.EscapeDataString(x.Key), "=", Uri.EscapeDataString(x.Value.ToString()))));
        }
    }
}
