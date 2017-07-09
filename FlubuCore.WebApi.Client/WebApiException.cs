using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using FlubuCore.WebApi.Model;

namespace FlubuCore.WebApi.Client
{
    public class WebApiException : Exception
    {
        public WebApiException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
