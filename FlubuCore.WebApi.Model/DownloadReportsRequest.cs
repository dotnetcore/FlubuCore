using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class DownloadReportsRequest
    {
        /// <summary>
        /// if <see cref="DownloadFromSubDirectory"/>  is not set whole root directory 'reports' on flubu server is packaged into zip and downloaded.
        /// If it is set then reports in specified subdirectory are zipped and downloaded 'Reports\{DownloadFromSubDirectory}'.
        /// </summary>
        public string DownloadFromSubDirectory { get; set; }
    }
}
