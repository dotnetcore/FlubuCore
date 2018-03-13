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

        /// <summary>
        /// if <see cref="UploadToSubDirectory"/>  is not set it is uploaded to the root directory 'packages'.
        /// If it is set then package is uploaded to specified sub directory 'Pacakges\{UploadToSubDirectory}'.
        /// </summary>
        public string UploadToSubDirectory { get; set; }

        public override string ToString()
        {
            return $"DirectoryPath: '{DirectoryPath}', PackageSeatchPattern: '{PackageSearchPattern}'";
        }
    }
}
