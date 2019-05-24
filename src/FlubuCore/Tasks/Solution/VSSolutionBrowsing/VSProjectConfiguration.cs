using System.Collections.Generic;

namespace FlubuCore.Tasks.Solution.VSSolutionBrowsing
{
    /// <summary>
    /// Contains information of compile configuration.
    /// </summary>
    public class VSProjectConfiguration
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

        public string Condition { get; set; }

        public IDictionary<string, string> Properties => _properties;
    }
}
