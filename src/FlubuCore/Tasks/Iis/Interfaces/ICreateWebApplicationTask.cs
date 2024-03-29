﻿namespace FlubuCore.Tasks.Iis.Interfaces
{
    public interface ICreateWebApplicationTask : ITaskOfT<int, ICreateWebApplicationTask>
    {
        ICreateWebApplicationTask LocalPath(string localPath);

        /// <summary>
        /// Application Name.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        ICreateWebApplicationTask ApplicationName(string applicationName);

        /// <summary>
        ///     Name of the application pool application will be controoler by.
        /// </summary>
        ICreateWebApplicationTask ApplicationPoolName(string applicationPoolName);

        ICreateWebApplicationTask ParentVirtualDirectoryName(string parentVirualDirectoryName);

        /// <summary>
        ///     Web site name web application will be added to.
        /// </summary>
        ICreateWebApplicationTask WebsiteName(string websiteName);

        /// <summary>
        ///     Mime types to be added.
        /// </summary>
        ICreateWebApplicationTask AddMimeType(params MimeType[] mimeTypes);

        ICreateWebApplicationTask ForServer(string serverName);
    }
}