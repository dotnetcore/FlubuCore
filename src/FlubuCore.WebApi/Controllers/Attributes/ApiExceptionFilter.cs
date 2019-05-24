using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers.Exceptions;
using FlubuCore.WebApi.Model;
using FlubuCore.WebApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlubuCore.WebApi.Controllers.Attributes
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        private readonly IRepositoryFactory _repositoryFactory;

        private WebApiSettings _webApiSettings;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IRepositoryFactory repositoryFactory,  IOptions<WebApiSettings> webApiOptions)
        {
            _webApiSettings = webApiOptions.Value;
            _logger = logger;
            _repositoryFactory = repositoryFactory;
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

            Thread.Sleep(2000);
            var logs = _repositoryFactory.CreateSerilogRepository().GetExecuteScriptLogs(context.HttpContext.TraceIdentifier);

            var error = new ErrorModel
            {
                ErrorCode = "InternalServerError",
                ErrorMessage = errorMessage,
                Logs = logs,
                StackTrace = _webApiSettings.IncludeStackTrace ? context.Exception.StackTrace : null
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
            else if (httpError.StatusCode == HttpStatusCode.InternalServerError)
            {
                Thread.Sleep(2000);
                var logs = _repositoryFactory.CreateSerilogRepository().GetExecuteScriptLogs(context.HttpContext.TraceIdentifier);
            }
            else
            {
                var error = new ErrorModel
                {
                    StatusCode = httpError.StatusCode,
                    ErrorCode = httpError.ErrorCode,
                    ErrorMessage = httpError.ErrorMessage,
                    ValidationErrors = httpError.ValidationErrors,
                    Logs = httpError.Logs
                };

                context.Result = new JsonResult(error);
            }
        }
    }
}
