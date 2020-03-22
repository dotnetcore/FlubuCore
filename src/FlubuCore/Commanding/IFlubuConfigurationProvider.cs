using System.Collections.Generic;

namespace FlubuCore.Commanding
{
    public interface IFlubuConfigurationProvider
    {
        Dictionary<string, string> GetConfiguration(string jsonSettingsFile);
    }
}
