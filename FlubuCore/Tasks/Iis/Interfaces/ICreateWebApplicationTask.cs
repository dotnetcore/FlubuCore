using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Tasks.Iis.Interfaces
{
    public interface ICreateWebApplicationTask : ITask
    {
        CreateWebApplicationMode Mode { get; set; }

        string ApplicationName { get; set; }

        string LocalPath { get; set; }

        bool AllowAnonymous { get; set; }

        bool AllowAuthNtlm { get; set; }

        string AnonymousUserName { get; set; }

        string AnonymousUserPass { get; set; }

        string AppFriendlyName { get; set; }

        string ApplicationPoolName { get; set; }

        bool AspEnableParentPaths { get; set; }

        bool AccessScript { get; set; }

        bool AccessExecute { get; set; }

        string DefaultDoc { get; set; }

        bool EnableDefaultDoc { get; set; }

        string ParentVirtualDirectoryName { get; set; }

        string WebsiteName { get; set; }

        IList<MimeType> MimeTypes { get; set; }
    }
}
