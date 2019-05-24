using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Tasks.Iis.Interfaces
{
    public interface IAddWebsiteBindingTask : ITaskOfT<int, IAddWebsiteBindingTask>
    {
        /// <summary>
        /// Add's binding.
        /// </summary>
        /// <param name="protocol">Binding protocol</param>
        /// <returns></returns>
        IAddWebsiteBindingTask AddBinding(string protocol);

        /// <summary>
        /// Web site name binding is added to.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IAddWebsiteBindingTask SiteName(string name);

        /// <summary>
        /// Certificate store.
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        IAddWebsiteBindingTask CertificateStore(string store);

        /// <summary>
        /// Hash of the certificate.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        IAddWebsiteBindingTask CertificateHash(string hash);
    }
}
