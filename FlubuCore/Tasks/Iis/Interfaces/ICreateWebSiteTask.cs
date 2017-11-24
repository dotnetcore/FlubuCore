using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using Microsoft.Web.Administration;

namespace FlubuCore.Tasks.Iis
{
    public interface ICreateWebsiteTask : ITaskOfT<int, ICreateWebsiteTask>
    {
        /// <summary>
        /// set the web site name.
        /// </summary>
        /// <param name="siteName">The web site name.</param>
        /// <returns>The <see cref="CreateWebsiteTask.CreateWebsiteBindingProtocol"/> instance.</returns>
        CreateWebsiteTask.CreateWebsiteBindingProtocol WebsiteName(string siteName);

        /// <summary>
        /// Set Website mode.
        /// </summary>
        /// <param name="value">The website Mode <see cref="CreateWebApplicationMode"/> </param>
        /// <returns>The Iis7CreateWebSiteTask.</returns>
        CreateWebsiteTask WebsiteMode(CreateWebApplicationMode value);

        /// <summary>
        /// Set web site application pool name.
        /// </summary>
        /// <param name="applicationPool">The application pool name</param>
        /// <returns>The  Iis7CreateWebSiteTask.</returns>
        CreateWebsiteTask ApplicationPoolName(string applicationPool);

        /// <summary>
        ///  Add MimeType. Can be used multiple times.
        /// </summary>
        /// <param name="mimeType">The mime type</param>
        /// <returns>The  Iis7CreateWebSiteTask.</returns>
        CreateWebsiteTask AddMimeType(MimeType mimeType);
    }
}
