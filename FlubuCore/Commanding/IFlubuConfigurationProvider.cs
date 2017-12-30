using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Cli.Flubu.Commanding
{
    public interface IFlubuConfigurationProvider
    {
        Dictionary<string, string> GetConfiguration(string jsonSettingsFile = "flubusettings.json");
    }
}
