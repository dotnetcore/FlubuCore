using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class UploadPackageRequest
    {
        /// <summary>
        /// The relative or absolute path to the directory to search. This string is not case-sensitive.
        /// </summary>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// The search string to match against the names of files in DirectoryPath.This parameter
        /// can contain a combination of valid literal path and wildcard (such as * and ?) characters, but doesn't support regular expressions.
        /// </summary>
        public string PackageSearchPattern { get; set; } 
    }
}
