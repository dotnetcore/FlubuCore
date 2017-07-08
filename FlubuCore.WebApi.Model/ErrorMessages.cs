using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class ErrorMessages
    {
        public const string InternalServerError = "Internal server error occured";

        public const string NotFound = "Resource not found.";

        public const string AccessDenied = "User tried to access to forbiden resource.";
    }
}
