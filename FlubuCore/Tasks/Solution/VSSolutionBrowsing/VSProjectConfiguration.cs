using System.Collections.Generic;

namespace FlubuCore.Tasks.Solution.VSSolutionBrowsing
{
    /// <summary>
    /// Contains information of compile configuration.
    /// </summary>
    public class VSProjectConfiguration
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

        private string _condition;

        public string Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public IDictionary<string, string> Properties
        {
            get { return _properties; }
        }
    }
}
