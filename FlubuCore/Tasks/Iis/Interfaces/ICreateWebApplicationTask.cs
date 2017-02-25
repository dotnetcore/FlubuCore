using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Tasks.Iis.Interfaces
{
    public interface ICreateWebApplicationTask : ITask
    {
        CreateWebApplicationMode Mode { get; set; }

        /// <summary>
        /// Name of the application.
        /// </summary>
        string ApplicationName { get; set; }

        string LocalPath { get; set; }

        bool AllowAnonymous { get; set; }

        bool AllowAuthNtlm { get; set; }

        string AnonymousUserName { get; set; }

        string AnonymousUserPass { get; set; }

        string AppFriendlyName { get; set; }

        /// <summary>
        /// Name of the application pool application will be controoler by.
        /// </summary>
        string ApplicationPoolName { get; set; }

        bool AspEnableParentPaths { get; set; }

        bool AccessScript { get; set; }

        bool AccessExecute { get; set; }

        string DefaultDoc { get; set; }

        bool EnableDefaultDoc { get; set; }

        string ParentVirtualDirectoryName { get; set; }

        /// <summary>
        /// Web site name web application will be added to.
        /// </summary>
        string WebsiteName { get; set; }

        /// <summary>
        /// Mime types to be added.
        /// </summary>
        IList<MimeType> MimeTypes { get; set; }
    }
}
