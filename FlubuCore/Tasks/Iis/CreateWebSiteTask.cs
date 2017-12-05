using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using Microsoft.Web.Administration;

namespace FlubuCore.Tasks.Iis
{
       public class CreateWebsiteTask : IisTaskBase<ICreateWebsiteTask>, ICreateWebsiteTask
    {
        /// <summary>
        /// Name of the website
        /// </summary>
        private string _webSiteName;

        /// <summary>
        /// The binding protocol. Http or https.
        /// </summary>
        private string _bindingProtocol;

        /// <summary>
        /// Port of the web application
        /// </summary>
        private int _port;

        /// <summary>
        /// Physical path to application.
        /// </summary>
        private string _physicalPath;

        private string _applicationPoolName = "DefaultAppPool";

        private IList<MimeType> _mimeTypes;

        private CreateWebApplicationMode _siteMode = CreateWebApplicationMode.DoNothingIfExists;
        private string _description;

        public CreateWebsiteTask()
        {
            _mimeTypes = new List<MimeType>();
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Creates web site {_webSiteName}";
                }

                return _description;
            }

            set { _description = value; }
        }

        public CreateWebsiteBindingProtocol WebsiteName(string siteName)
        {
            _webSiteName = siteName;
            return new CreateWebsiteBindingProtocol(this);
        }

        public CreateWebsiteTask WebsiteMode(CreateWebApplicationMode value)
        {
            _siteMode = value;
            return this;
        }

        public CreateWebsiteTask ApplicationPoolName(string applicationPool)
        {
            _applicationPoolName = applicationPool;
            return this;
        }

        public CreateWebsiteTask AddMimeType(MimeType mimeType)
        {
            _mimeTypes.Add(mimeType);
            return this;
        }

        /// <summary>
        /// Creates or updated the web site.
        /// </summary>
        /// <param name="context">The task context</param>
        protected override int DoExecute(ITaskContextInternal context)
        {
            Validate();
            using (ServerManager serverManager = new ServerManager())
            {
                var webSiteExists = WebsiteExists(serverManager, _webSiteName);

                if (_siteMode == CreateWebApplicationMode.DoNothingIfExists && webSiteExists)
                {
                    return 0;
                }

                if (_siteMode == CreateWebApplicationMode.FailIfAlreadyExists && webSiteExists)
                {
                    throw new TaskExecutionException(string.Format(CultureInfo.InvariantCulture, "web site {0} already exists!", _webSiteName), 1);
                }

                if (_siteMode == CreateWebApplicationMode.UpdateIfExists && webSiteExists)
                {
                   serverManager.Sites[_webSiteName].Delete();
                }

                var bindingInformation = string.Format(CultureInfo.InvariantCulture, "*:{0}:", _port);

                var site = serverManager.Sites.Add(_webSiteName, _bindingProtocol, bindingInformation, _physicalPath);
                site.ApplicationDefaults.ApplicationPoolName = _applicationPoolName;
                serverManager.CommitChanges();

                Microsoft.Web.Administration.Configuration config = site.GetWebConfiguration();
                AddMimeTypes(config, _mimeTypes);
                serverManager.CommitChanges();
            }

            return 0;
        }

        /// <summary>
        /// Validates Ii7CreateWebSiteTask properties.
        /// </summary>
        private void Validate()
        {
            if (string.IsNullOrEmpty(_webSiteName))
            {
                throw new TaskExecutionException("webSiteName missing!", 1);
            }

            if (string.IsNullOrEmpty(_bindingProtocol))
            {
                throw new TaskExecutionException("bindingProtocol missing!", 1);
            }

            if (_port == 0)
            {
                throw new TaskExecutionException("Port missing!", 1);
            }

            if (string.IsNullOrEmpty(_physicalPath))
            {
                throw new TaskExecutionException("physicalPath missing!", 1);
            }

            if (!_bindingProtocol.Equals("http", StringComparison.OrdinalIgnoreCase)
                && !_bindingProtocol.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                throw new TaskExecutionException("Wrong binding protocol. Supported values http and https", 1);
            }
        }

        public class CreateWebsiteBindingProtocol
        {
            private readonly CreateWebsiteTask _task;

            public CreateWebsiteBindingProtocol(CreateWebsiteTask task)
            {
                _task = task;
            }

            /// <summary>
            /// Sets the binding protocol. Http or https.
            /// </summary>
            /// <param name="value">The binding protocol. Supported values http and https.</param>
            /// <returns>new instance of <see cref="CreateWebsiteProtocol"/></returns>
            public CreateWebsiteProtocol BindingProtocol(string value)
            {
                _task._bindingProtocol = value;
                return new CreateWebsiteProtocol(_task);
            }
        }

        public class CreateWebsiteProtocol
        {
            private readonly CreateWebsiteTask _task;

            public CreateWebsiteProtocol(CreateWebsiteTask task)
            {
                _task = task;
            }

            /// <summary>
            /// Sets the port of the web site.
            /// </summary>
            /// <param name="value">The port</param>
            /// <returns>New instance of <see cref="CreateWebsitePhysicalPath"/></returns>
            public CreateWebsitePhysicalPath Port(int value)
            {
                _task._port = value;
                return new CreateWebsitePhysicalPath(_task);
            }
        }

        public class CreateWebsitePhysicalPath
        {
            private readonly CreateWebsiteTask _task;

            public CreateWebsitePhysicalPath(CreateWebsiteTask task)
            {
                _task = task;
            }

            /// <summary>
            /// Sets the physical path to the web site.
            /// </summary>
            /// <param name="value">The physical path.</param>
            /// <returns>The iI7CreateWebSiteTask.</returns>
            public CreateWebsiteTask PhysicalPath(string value)
            {
                _task._physicalPath = value;
                return _task;
            }
        }
    }
}
