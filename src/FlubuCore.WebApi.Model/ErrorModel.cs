using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class ErrorModel
    {
        /// <summary>
        /// Http status code of the error.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The error code.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Stack trace of the exception.
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Flubu web api logs..
        /// </summary>
        public List<string> Logs { get; set; }

        /// <summary>
        /// Validation errors.
        /// </summary>
        public List<ValidationError> ValidationErrors { get; set; }
    }
}
