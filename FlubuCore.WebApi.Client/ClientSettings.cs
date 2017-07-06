using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.WebApi.Client
{
    public class ClientSettings
    {
        private int timeout = 5000;

        /// <summary>
        /// Base url to simobil web api (without /api).
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The request timeout in miliseconds.
        /// </summary>
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }
    }
}
