using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Context.Attributes.BuildProperties
{
    /// <summary>
    ///  Get or sets <see cref="BuildVersion"/> from build properties session.
    /// </summary>
    public class BuildVersionAttribute : BuildPropertyAttribute
    {
        public BuildVersionAttribute()
            : base(BuildProps.BuildVersion)
        {
        }
    }
}
