using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Tasks.Iis.Interfaces
{
    public interface IAddWebsiteBindingTask : ITask
    {
        IAddWebsiteBindingTask AddBinding(string protocol);

        IAddWebsiteBindingTask SiteName(string name);

        IAddWebsiteBindingTask CertificateStore(string store);

        IAddWebsiteBindingTask CertificateHash(string hash);
    }
}
