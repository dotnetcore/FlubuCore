using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Controllers.Exception;
using FlubuCore.WebApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;

namespace FlubuCore.WebApi.Controllers.Attributes
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
           _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is HttpError)
            {
                HandleHttpError(context);
            }
            else
            {
                HandleInternalServerError(context);
            }

            base.OnException(context);
        }

        private void HandleInternalServerError(ExceptionContext context)
        {
            _logger.LogError("Exception occured: {0}", context.Exception);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            string errorMessage;
            if (context.Exception is TaskExecutionException)
            {
                errorMessage = context.Exception.Message;
            }
            else
            {
                errorMessage = ErrorMessages.InternalServerError;
            }

            var error = new ErrorModel
            {
                ErrorCode = "InternalServerError",
                ErrorMessage = errorMessage
            };

            context.Result = new JsonResult(error);
        }

        private void HandleHttpError(ExceptionContext context)
        {
            _logger.LogWarning($"HttpError occured: {0}", context.Exception);
            var httpError = (HttpError)context.Exception;
            context.HttpContext.Response.StatusCode = (int)httpError.StatusCode;
            if (httpError.StatusCode == HttpStatusCode.NotFound && string.IsNullOrEmpty(httpError.ErrorCode))
            {
                var error = new ErrorModel
                {
                    ErrorCode = ErrorCodes.NotFound,
                    ErrorMessage = ErrorMessages.NotFound
                };

                context.Result = new JsonResult(error);
            }
            else
            {
                var error = new ErrorModel
                {
                    StatusCode = httpError.StatusCode,
                    ErrorCode = httpError.ErrorCode,
                    ErrorMessage = httpError.ErrorMessage,
                    ValidationErrors = httpError.ValidationErrors,
                };

                context.Result = new JsonResult(error);
            }
        }
    }
}
