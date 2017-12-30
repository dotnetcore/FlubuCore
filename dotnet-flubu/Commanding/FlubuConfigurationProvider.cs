using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore;
using Microsoft.Extensions.Configuration;

namespace DotNet.Cli.Flubu.Commanding
{
    public class FlubuConfigurationProvider
    {
        public Dictionary<string, string> BuildConfiguration(string jsonSettingsFile = "flubusettings.json")
        {
            try
            {
                var builder = new ConfigurationBuilder();
                var config = builder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddEnvironmentVariables("flubu_")
                    .AddJsonFile(jsonSettingsFile, optional: true)
                    .Build();

                Dictionary<string, string> dic = new Dictionary<string, string>();
                config.Bind(dic);
                return dic;
            }
            catch (InvalidOperationException e)
            {
                throw new FlubuConfigurationException("Flubu supports only simple key/value JSON configuration.", e);
            }
        }
    }
}
