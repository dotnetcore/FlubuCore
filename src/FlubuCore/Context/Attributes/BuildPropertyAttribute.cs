using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context.Attributes
{
    /// <summary>
    ///  Get or sets specified build property from build properties session.
    /// </summary>
    public class BuildPropertyAttribute : Attribute
    {
        /// <summary>
        /// Get or sets specified build property from build properties session.
        /// </summary>
        /// <param name="buildProperty">The build property. Use <see cref="BuildProps"/> constants.</param>
        public BuildPropertyAttribute(string buildProperty)
        {
            BuildProperty = buildProperty;
        }

        public BuildPropertyAttribute(PredefinedBuildProperties buildProperty)
        {
            BuildProperty = buildProperty.ToString();
        }

        /// <summary>
        /// The build property. Use <see cref="BuildProps"/> constants.
        /// </summary>
        public string BuildProperty { get; }
    }
}
