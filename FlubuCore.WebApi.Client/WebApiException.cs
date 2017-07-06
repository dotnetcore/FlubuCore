using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FlubuCore.WebApi.Client
{
    public class WebApiException : Exception
    {
        public WebApiException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }
    }
}
