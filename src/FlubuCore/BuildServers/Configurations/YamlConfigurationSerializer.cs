using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlubuCore.BuildServers.Configurations
{
    public class YamlConfigurationSerializer
    {
        public string Serialize<T>(T item)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .IgnoreFields()
                .Build();

            return serializer.Serialize(item);
        }
    }
}
