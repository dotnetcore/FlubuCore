using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context.Attributes.BuildProperties
{
    /// <summary>
    ///  Get or sets product id from build properties session.
    /// </summary>
    public class ProductIdAttribute : BuildPropertyAttribute
    {
        public ProductIdAttribute()
            : base(BuildProps.ProductId)
        {
        }
    }
}
