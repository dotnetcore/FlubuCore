using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.FlubuWebApi;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface IWebApiFluentInterface
    {
        /// <summary>
        /// Upload's sprecified packages to flubu web api server.
        /// </summary>
        /// <param name="directoryPath">The relative or absolute path to the directory where packages are searched.</param>
        /// <param name="packageSearchPattern">The search string to match against the names of files(packages). This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
        /// <returns></returns>
        UploadPackageTask UploadPackageTask(string directoryPath, string packageSearchPattern);

        /// <summary>
        /// Executes specified flubu scrip on flubu web api server.
        /// </summary>
        /// <param name="mainCommand">Command to be executed.</param>
        /// <param name="scriptFilePath">Location to the flubu script on the flubu web api server.</param>
        /// <returns></returns>
        ExecuteFlubuScriptTask ExecuteScriptTask(string mainCommand, string scriptFilePath);

        /// <summary>
        /// Get's the token that can access flubu web api server.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        GetTokenTask GetTokenTask(string username, string password);

        /// <summary>
        /// Deletes all packages(cleans directory on flubu web api server).
        /// </summary>
        /// <returns></returns>
        DeletePackagesTask DeletePackagesTask();

        /// <summary>
        /// Uploads flubu script to flubu web api server.
        /// </summary>
        /// <param name="scriptFilePath"> The relative or absolute path to the flubu script to be uploaded to web api server.</param>
        /// <returns></returns>
        UploadScriptTask UploadScriptTask(string scriptFilePath);

        /// <summary>
        /// Deletes all reports(cleans directory on flubu web api server).
        /// </summary>
        /// <returns></returns>
        DeleteReportsTask DeleteReportsTask();

        /// <summary>
        /// Download reports(compressed in zip file) from flubu web api server.
        /// </summary>
        /// <param name="saveAs">name of the file (path) that the reports will be saved to.</param>
        /// <returns></returns>
        DownloadReportsTask DownloadReportsTask(string saveAs);
    }
}
