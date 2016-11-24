using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Tasks.Iis.Interfaces;
using Microsoft.Web.Administration;

namespace FlubuCore.Tasks.Iis
{
    public class CreateWebApplicationTask : IisTaskBase, ICreateWebApplicationTask
    {
        private string _localPath;

        public CreateWebApplicationMode Mode { get; set; } = CreateWebApplicationMode.FailIfAlreadyExists;

        public string ApplicationName { get; set; }

        public string LocalPath
        {
            get { return _localPath; }
            set { _localPath = Path.GetFullPath(value); }
        }

        public bool AllowAnonymous { get; set; } = true;

        public bool AllowAuthNtlm { get; set; } = true;

        public string AnonymousUserName { get; set; }

        public string AnonymousUserPass { get; set; }

        public string AppFriendlyName { get; set; }

        public bool AspEnableParentPaths { get; set; }

        public bool AccessScript { get; set; } = true;

        public bool AccessExecute { get; set; }

        public string DefaultDoc { get; set; }

        public bool EnableDefaultDoc { get; set; } = true;

        /// <summary>
        /// Gets or sets the Name of the website that the web application is added too. By default it is "Default Web Site"
        /// </summary>
        public string WebsiteName { get; set; } = "Default Web Site";

        public string ParentVirtualDirectoryName { get; set; } = @"IIS://localhost/W3SVC/1/Root";

        public string ApplicationPoolName { get; set; } = "DefaultAppPool";

        public IList<MimeType> MimeTypes { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (string.IsNullOrEmpty(ApplicationName))
            {
                throw new TaskExecutionException("ApplicationName missing!", 1);
            }

            using (ServerManager serverManager = new ServerManager())
            {
                if (!WebsiteExists(serverManager, WebsiteName))
                {
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, "Web site '{0}' does not exists.", WebsiteName));
                }

                Site site = serverManager.Sites[WebsiteName];

                string vdirPath = "/" + ApplicationName;
                foreach (Application application in site.Applications)
                {
                    if (application.Path == vdirPath)
                    {
                        if (Mode == CreateWebApplicationMode.DoNothingIfExists)
                        {
                            context.LogInfo($"Web application '{ApplicationName}' already exists, doing nothing.");
                            return 0;
                        }

                        if (Mode == CreateWebApplicationMode.FailIfAlreadyExists)
                        {
                            throw new TaskExecutionException(
                                string.Format(
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    "Web application '{0}' already exists.",
                                    ApplicationName), 0);
                        }

                        //// otherwise we should update the existing virtual directory
                        ////TODO update existing application
                        ////ourApplication = application;
                        return 0;
                    }
                }

                using (ServerManager manager = new ServerManager())
                {
                    Site defaultSite = manager.Sites[WebsiteName];
                    Application ourApplication = defaultSite.Applications.Add(vdirPath, LocalPath);
                    ourApplication.ApplicationPoolName = ApplicationPoolName;
                    var config = ourApplication.GetWebConfiguration();
                    AddMimeTypes(config, MimeTypes);
                    manager.CommitChanges();
                }
            }

            return 0;
        }
    }
}
