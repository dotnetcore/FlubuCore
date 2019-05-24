using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Tasks.Iis.Interfaces;
using Microsoft.Web.Administration;

namespace FlubuCore.Tasks.Iis
{
    using NuGet.Packaging;

    public class CreateWebApplicationTask : IisTaskBase<ICreateWebApplicationTask>, ICreateWebApplicationTask
    {
        private readonly IList<MimeType> _mimeTypes;
        private string _applicationName;
        private string _localPath;
        private string _websiteName = "Default Web Site";
        private string _applicationPoolName = "DefaultAppPool";
        private CreateWebApplicationMode _mode = CreateWebApplicationMode.FailIfAlreadyExists;
        private string _description;

        public CreateWebApplicationTask(string applicationName)
        {
            _mimeTypes = new List<MimeType>();
            _applicationName = applicationName;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Creates web application {_applicationName}";
                }

                return _description;
            }

            set => _description = value;
        }

        [Obsolete("Use CrearteWebAppMode instead")]
        public ICreateWebApplicationTask Mode(CreateWebApplicationMode mode)
        {
            _mode = mode;
            return this;
        }

        public ICreateWebApplicationTask WebAppMode(CreateWebApplicationMode mode)
        {
            _mode = mode;
            return this;
        }

        public ICreateWebApplicationTask LocalPath(string localPath)
        {
            _localPath = Path.GetFullPath(localPath);
            return this;
        }

        public ICreateWebApplicationTask ApplicationName(string applicationName)
        {
            _applicationName = applicationName;
            return this;
        }

        public ICreateWebApplicationTask ParentVirtualDirectoryName(string parentVirualDirectoryName)
        {
            return this;
        }

        /// <summary>
        /// Gets or sets the Name of the website that the web application is added too. By default it is "Default Web Site"
        /// </summary>
        public ICreateWebApplicationTask WebsiteName(string websiteName)
        {
            _websiteName = websiteName;
            return this;
        }

        public ICreateWebApplicationTask ApplicationPoolName(string applicationPoolName)
        {
            _applicationPoolName = applicationPoolName;
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
                if (!WebsiteExists(serverManager, _websiteName))
                {
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, "Web site '{0}' does not exists.", _websiteName));
                }

                Site site = serverManager.Sites[_websiteName];

                string vdirPath = "/" + _applicationName;
                foreach (Application application in site.Applications)
                {
                    if (application.Path == vdirPath)
                    {
                        if (_mode == CreateWebApplicationMode.DoNothingIfExists)
                        {
                            DoLogInfo($"Web application '{_applicationName}' already exists, doing nothing.");
                            return 0;
                        }

                        if (_mode == CreateWebApplicationMode.FailIfAlreadyExists)
                        {
                            throw new TaskExecutionException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Web application '{0}' already exists.",
                                    _applicationName), 0);
                        }

                        //// otherwise we should update the existing virtual directory
                        ////TODO update existing application
                        ////ourApplication = application;
                        return 0;
                    }
                }

                using (ServerManager manager = new ServerManager())
                {
                    Site defaultSite = manager.Sites[_websiteName];
                    Application ourApplication = defaultSite.Applications.Add(vdirPath, _localPath);
                    ourApplication.ApplicationPoolName = _applicationPoolName;
                    var config = ourApplication.GetWebConfiguration();
                    AddMimeTypes(config, _mimeTypes);
                    manager.CommitChanges();
                }
            }

            return 0;
        }
    }
}
