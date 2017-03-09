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
    using NuGet.Packaging;

    public class CreateWebApplicationTask : IisTaskBase, ICreateWebApplicationTask
    {
        private string _localPath;

        private bool allowAnonymous = true;

        private bool allowAuthNtlm = true;

        private string anonymousUserName;

        private string anonymousUserPass;

        private string appFriendlyName;

        private bool aspEnableParentPaths;

        private bool accessScript = true;

        private bool accessExecute;

        private string defaultDoc;

        private bool enableDefaultDoc = true;

        private string websiteName = "Default Web Site";

        private string parentVirtualDirectoryName = @"IIS://localhost/W3SVC/1/Root";

        private string applicationPoolName = "DefaultAppPool";

        private IList<MimeType> mimeTypes;

        private CreateWebApplicationMode mode = CreateWebApplicationMode.FailIfAlreadyExists;

        private string applicationName;

        public CreateWebApplicationTask(string applicationName)
        {
            this.mimeTypes = new List<MimeType>();
            this.applicationName = applicationName;
        }

        public ICreateWebApplicationTask Mode(CreateWebApplicationMode mode)
        {
            this.mode = mode;
            return this;
        }

        public ICreateWebApplicationTask LocalPath(string localPath)
        {
            this._localPath = Path.GetFullPath(localPath);
            return this;
        }

        public ICreateWebApplicationTask AllowAnonymous()
        {
            this.allowAnonymous = true;
            return this;
        }

        public ICreateWebApplicationTask AllowAuthNtlm()
        {
            this.allowAuthNtlm = true;
            return this;
        }

        public ICreateWebApplicationTask AnonymousUserName(string anonymousUsername)
        {
            this.anonymousUserName = anonymousUsername;
            return this;
        }

        public ICreateWebApplicationTask AnonymousUserPass(string anonymousUserPass)
        {
            this.anonymousUserPass = anonymousUserPass;
            return this;
        }

        public ICreateWebApplicationTask AppFriendlyName(string appFriendlyName)
        {
            this.appFriendlyName = appFriendlyName;
            return this;
        }

        public ICreateWebApplicationTask AspEnableParentPaths()
        {
            this.aspEnableParentPaths = true;
            return this;
        }

        public ICreateWebApplicationTask AccessScript()
        {
            this.accessScript = true;
            return this;
        }

        public ICreateWebApplicationTask AccessExecute()
        {
            this.accessExecute = true;
            return this;
        }

        public ICreateWebApplicationTask DefaultDoc(string defaultDoc)
        {
            this.defaultDoc = defaultDoc;
            return this;
        }

        public ICreateWebApplicationTask EnableDefaultDoc()
        {
            this.enableDefaultDoc = true;
            return this;
        }

        /// <summary>
        /// Gets or sets the Name of the website that the web application is added too. By default it is "Default Web Site"
        /// </summary>
        public ICreateWebApplicationTask WebsiteName(string websiteName)
        {
            this.websiteName = websiteName;
            return this;
        }

        public ICreateWebApplicationTask ParentVirtualDirectoryName(string parentVirualDirectoryName)
        {
            this.parentVirtualDirectoryName = parentVirualDirectoryName;
            return this;
        }

        public ICreateWebApplicationTask ApplicationPoolName(string applicationPoolName)
        {
            this.applicationPoolName = applicationPoolName;
            return this;
        }

        public ICreateWebApplicationTask AddMimeType(params string[] mimeTypes)
        {
            mimeTypes.AddRange(mimeTypes);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                if (!WebsiteExists(serverManager, websiteName))
                {
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, "Web site '{0}' does not exists.", websiteName));
                }

                Site site = serverManager.Sites[websiteName];

                string vdirPath = "/" + applicationName;
                foreach (Application application in site.Applications)
                {
                    if (application.Path == vdirPath)
                    {
                        if (mode == CreateWebApplicationMode.DoNothingIfExists)
                        {
                            context.LogInfo($"Web application '{applicationName}' already exists, doing nothing.");
                            return 0;
                        }

                        if (mode == CreateWebApplicationMode.FailIfAlreadyExists)
                        {
                            throw new TaskExecutionException(
                                string.Format(
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    "Web application '{0}' already exists.",
                                    applicationName), 0);
                        }

                        //// otherwise we should update the existing virtual directory
                        ////TODO update existing application
                        ////ourApplication = application;
                        return 0;
                    }
                }

                using (ServerManager manager = new ServerManager())
                {
                    Site defaultSite = manager.Sites[websiteName];
                    Application ourApplication = defaultSite.Applications.Add(vdirPath, _localPath);
                    ourApplication.ApplicationPoolName = applicationPoolName;
                    var config = ourApplication.GetWebConfiguration();
                    AddMimeTypes(config, mimeTypes);
                    manager.CommitChanges();
                }
            }

            return 0;
        }
    }
}
