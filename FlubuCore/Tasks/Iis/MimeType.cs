using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Tasks.Iis
{
    public class MimeType
    {
        /// <summary>
        /// Gets or sets File extension of the mime type.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets The Mime type.
        /// </summary>
        public string MimeTypeName { get; set; }
    }
}
