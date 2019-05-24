using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Model;
using FluentValidation;

namespace FlubuCore.WebApi.Validators
{
    public class ExecuteScriptRequestValidator : AbstractValidator<ExecuteScriptRequest>
    {
        public ExecuteScriptRequestValidator()
        {
            RuleFor(x => x.TargetToExecute).NotEmpty();
        }
    }
}
