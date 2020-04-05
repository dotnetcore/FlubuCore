using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class ErrorCodes
    {
        public const string InternalServerError = "InternalServerError";

        /// <summary>
        /// Occures when request validation has failed.
        /// </summary>
        public const string BadRequest = "BadRequest";

        /// <summary>
        /// Occures when resource is not found.
        /// </summary>
        public const string NotFound = "NotFound";

        /// <summary>
        /// Occures when script is not found.
        /// </summary>
        public const string ScriptNotFound = "ScriptNotFound";

        public const string TargetNotFound = "TargetNotFound";

        /// <summary>
        /// Occures when user doesnt have access to specific resource.
        /// </summary>
        public const string Forbiden = "Forbiden";

        public const string ForbidenIp = "ForbidenIp";

        public const string ModelStateNotValid = "ModelStateNotValid";
    }
}
