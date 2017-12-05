namespace FlubuCore.Context
{
    public class BuildPropertiesContext : IBuildPropertiesContext
    {
        public BuildPropertiesContext(IBuildPropertiesSession properties)
        {
            Properties = properties;
            ScriptArgs = new DictionaryWithDefault<string, string>(null);
        }

        public IBuildPropertiesSession Properties { get; }

        public DictionaryWithDefault<string, string> ScriptArgs { get; set; }
    }
}
