using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class CleanReportsDirectoryRequest
    {
        /// <summary>
        /// if <see cref="SubDirectoryToDelete"/>  is not set root directory 'Reports' and subfolders are cleaned.
        /// If it is set then  specified sub directory is cleaned 'Reports\{UploadToSubDirectory}'.
        /// </summary>
        public string SubDirectoryToDelete { get; set; }
    }
}
