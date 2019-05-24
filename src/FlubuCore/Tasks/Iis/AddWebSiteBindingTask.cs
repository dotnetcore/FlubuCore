using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Tasks.Iis.Interfaces;
using Microsoft.Web.Administration;

namespace FlubuCore.Tasks.Iis
{
    public class AddWebsiteBindingTask : TaskBase<int, IAddWebsiteBindingTask>, IAddWebsiteBindingTask
    {
        private string _siteName;

        private string _bindProtocol;

        private string _certificateStore;

        private string _certificateHash;

        protected override string Description { get; set; }

        public IAddWebsiteBindingTask SiteName(string name)
        {
            _siteName = name;
            return this;
        }

        public IAddWebsiteBindingTask AddBinding(string protocol)
        {
            _bindProtocol = protocol;
            return this;
        }

        public IAddWebsiteBindingTask CertificateStore(string store)
        {
            _certificateStore = store;
            return this;
        }

        public IAddWebsiteBindingTask CertificateHash(string hash)
        {
            _certificateHash = hash;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (string.IsNullOrEmpty(_siteName))
                throw new TaskExecutionException("Site name missing!", 1);
            if (string.IsNullOrEmpty(_bindProtocol))
                throw new TaskExecutionException("Protocol missing!", 1);
            if (_bindProtocol.IndexOf("https", StringComparison.OrdinalIgnoreCase) >= 0 &&
                (string.IsNullOrEmpty(_certificateStore) || string.IsNullOrEmpty(_certificateHash)))
                throw new TaskExecutionException("Certificate store or hash not set for SSL protocol", 1);

            using (ServerManager manager = new ServerManager())
            {
                Site site = manager.Sites[_siteName];

                //// See if this binding is already on some site
                if (manager.Sites.Where(st => st.Bindings.Where(b => b.Protocol == _bindProtocol).Any()).Any())
                {
                    DoLogInfo($"Binding for protocol '{_bindProtocol}' already exists! Doing nothing.");
                    return 0;
                }

                Binding binding = site.Bindings.CreateElement();
                binding.Protocol = _bindProtocol;
                binding.CertificateStoreName = _certificateStore;
                binding.CertificateHash = Encoding.UTF8.GetBytes(_certificateHash);
                site.Bindings.Add(binding);

                manager.CommitChanges();
            }

            return 0;
        }
    }
}
