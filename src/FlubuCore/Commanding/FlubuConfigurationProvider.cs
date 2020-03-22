using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FlubuCore.Commanding
{
    public class FlubuConfigurationProvider : IFlubuConfigurationProvider
    {
        public Dictionary<string, string> GetConfiguration(string jsonSettingsFile)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddEnvironmentVariables("flubu_")
                    .AddJsonFile(jsonSettingsFile, optional: true);

                string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (!string.IsNullOrEmpty(environment))
                {
                    var jsonSettingsEnvFile = Path.GetFileNameWithoutExtension(jsonSettingsFile);
                    jsonSettingsEnvFile = $"{jsonSettingsEnvFile}.{environment}.json";
                    builder.AddJsonFile(jsonSettingsEnvFile, optional: true);
                }

                var jsonSettingsByMachineNameFile = Path.GetFileNameWithoutExtension(jsonSettingsFile);
                jsonSettingsByMachineNameFile = $"{jsonSettingsByMachineNameFile}.{Environment.MachineName}.json";
                builder.AddJsonFile(jsonSettingsByMachineNameFile, optional: true);

                var config = builder.Build();

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
