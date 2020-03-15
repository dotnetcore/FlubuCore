using FlubuCore.Tasks.Solution.VSSolutionBrowsing;

namespace FlubuCore.Context.Attributes.BuildProperties
{
    /// <summary>
    /// Get or sets <see cref="VSSolution"/> from build properties session.
    /// </summary>
    public class VsSolutionAttribute : BuildPropertyAttribute
    {
        public VsSolutionAttribute()
            : base(BuildProps.Solution)
        {
        }
    }
}
