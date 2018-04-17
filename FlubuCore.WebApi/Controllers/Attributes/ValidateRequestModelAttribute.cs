using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using FlubuCore.WebApi.Controllers.Exceptions;
using FlubuCore.WebApi.Model;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlubuCore.WebApi.Controllers.Attributes
{
    public class ValidateRequestModelAttribute : ActionFilterAttribute
    {
        private readonly IValidatorFactory _validatorFactory;

        public ValidateRequestModelAttribute(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
            Order = 3;
        }

        /// <summary>
        /// Validate the model before control passes to the controller's action
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ActionArguments.ContainsKey("request"))
            {
                return;
            }

            var request = context.ActionArguments["request"];
            var validator = _validatorFactory.GetValidator(request.GetType());

            if (validator == null)
            {
                return;
            }

            ValidationResult validationResult = validator.Validate(request);
            if (validationResult.IsValid)
            {
                return;
            }

            List<ValidationError> errors = new List<ValidationError>();

            foreach (var result in validationResult.Errors)
            {
                var error = new ValidationError();
                error.PropertyName = result.PropertyName;
                error.ErrorCode = result.ErrorCode;
                error.ErrorMessage = result.ErrorMessage;
                errors.Add(error);
            }

            var httpError = new HttpError(HttpStatusCode.BadRequest, ErrorCodes.ModelStateNotValid, "ModelState is not valid.");
            httpError.ValidationErrors = errors;
            throw httpError;
        }
    }
}
