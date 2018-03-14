using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class CleanPackagesDirectoryRequest
    {
        /// <summary>
        /// if <see cref="SubDirectoryToDelete"/>  is not set root directory 'packages' and subfolders are cleaned.
        /// If it is set then  specified sub directory is cleaned 'Pacakges\{UploadToSubDirectory}'.
        /// </summary>
        public string SubDirectoryToDelete { get; set; }
    }
}
