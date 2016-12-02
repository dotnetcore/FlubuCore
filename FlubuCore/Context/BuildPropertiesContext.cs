namespace FlubuCore.Context
{
    public class BuildPropertiesContext : IBuildPropertiesContext
    {
        public BuildPropertiesContext(IBuildPropertiesSession properties)
        {
            Properties = properties;
        }

        public IBuildPropertiesSession Properties { get; }
    }
}
