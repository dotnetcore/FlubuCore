using System.Collections.Generic;
using System.Net;
using FlubuCore.WebApi.Model;

namespace FlubuCore.WebApi.Controllers.Exception
{
    public class HttpError : System.Exception
    {
        public HttpError(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpError(HttpStatusCode statusCode, string errorCode)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public HttpError(HttpStatusCode statusCode, string errorCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public override string Message
        {
            get
            {
                string validationErrorMessages = null;
                if (ValidationErrors != null && ValidationErrors.Count > 0)
                {
                    validationErrorMessages = "validationErrors: ";
                    foreach (var validationError in ValidationErrors)
                    {
                        validationErrorMessages = string.Format("{0}, '{1}'", validationErrorMessages, validationError.ErrorMessage);
                    }
                }

                return string.Format("HttpStatusCode: '{0}', ErrorCode: '{1}', ErrorMessage: '{2}', {4}", StatusCode, ErrorCode, ErrorMessage, validationErrorMessages);
            }
        }

        public HttpStatusCode StatusCode { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string LocalizedMessage { get; set; }

        public List<ValidationError> ValidationErrors { get; set; }

        public static HttpError InternalServerError(string errorCode, string errorMessage)
        {
            throw new HttpError(HttpStatusCode.InternalServerError, errorMessage);
        }

        public static HttpError BadRequest(string errorCode, string errorMessage)
        {
            return new HttpError(HttpStatusCode.BadRequest, errorMessage);
        }

        public static HttpError Forbiden(string errorCode, string errorMessage)
        {
            return new HttpError(HttpStatusCode.Forbidden, errorCode, errorMessage);
        }

        public static HttpError NotFound()
        {
            return new HttpError(HttpStatusCode.NotFound);
        }
    }
}