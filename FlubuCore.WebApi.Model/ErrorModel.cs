using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class ErrorModel
    {
        public HttpStatusCode StatusCode { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string StackTrace { get; set; }

        public List<ValidationError> ValidationErrors { get; set; }
    }
}
