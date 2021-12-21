using System;
using System.Collections.Generic;
using System.Linq;
using Bridge.Commons.System.Exceptions;
using Bridge.Commons.System.Models.Validations;
using Bridge.Commons.Validation.Contracts;
using Bridge.Commons.Validation.Enums;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Bridge.Commons.System.AspNet.Filters
{
    /// <summary>
    ///     Validação de filtro
    /// </summary>
    public class ValidationFilter : ActionFilterAttribute
    {
        /// <summary>
        ///     Validação durante a ação sendo executada
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="RequestException"></exception>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var parameters = context.ActionArguments;

            IList<ValidationResult> results =
            (
                from parameter in parameters
                let validator = FindValidator(parameter.Value)
                where validator != null
                select validator.Validate(new ValidationContext<object>(parameter.Value))
                into validationResult
                where !validationResult.IsValid
                select validationResult
            ).ToList();

            if (results.Count > 0)
                throw Create<RequestException>(results);
        }

        private static IValidator FindValidator(object request)
        {
            return (request as IValidationRequest)?.GetValidator();
        }

        private static TException Create<TException>(IEnumerable<ValidationResult> results)
            where TException : BaseException, new()
        {
            IList<ErrorInstance> errors = new List<ErrorInstance>();
            errors = results.Aggregate(errors, (current, result) => current.Concat(result.Errors.Select(x =>
                {
                    var error = !(x.CustomState is IDictionary<EValidationField, string> custom)
                        ? new ErrorInstance(-1, x.ErrorMessage, x.PropertyName)
                        : new ErrorInstance(Convert.ToInt32(custom[EValidationField.ERROR_CODE]),
                            custom[EValidationField.MESSAGE], x.PropertyName);

                    return error;
                }))
                .ToList());

            var exception = new TException();
            exception.Initialize(errors);

            return exception;
        }
    }
}