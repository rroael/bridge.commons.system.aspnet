using System;
using System.Net;
using System.Threading.Tasks;
using Bridge.Commons.System.Exceptions;
using Bridge.Commons.System.Models.Validations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Bridge.Commons.System.AspNet.Middlewares
{
    /// <summary>
    ///     Erro executandoo Middleware
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private static readonly JsonSerializerSettings JsonSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };

        private readonly string _errorMessageTemplate = "{@ExceptionMessage} {@RequestHeaders} {@RequestBody}";
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        /// <summary>
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (RequestException e)
            {
                //exceptions de validações formatos/dados das requisições
                await HandleExceptionAsync(context, e);
            }
            catch (AuthenticationException e)
            {
                await HandleExceptionAsync(context, e);
            }
            catch (PermissionException e)
            {
                await HandleExceptionAsync(context, e);
            }
            catch (ConflictException e)
            {
                await HandleExceptionAsync(context, e);
            }
            catch (BusinessException e)
            {
                //exceptions de validações de regras de negócios não precisam ser logadas
                await HandleExceptionAsync(context, e);
            }
            catch (RepositoryException e)
            {
                _logger.Error(e, _errorMessageTemplate, e.Message, context.Request.Headers, context.Request.Body);
                await HandleExceptionAsync(context, e);
            }
            catch (NotFoundException e)
            {
                await HandleExceptionAsync(context, e);
            }
            catch (Exception e)
            {
                _logger.Error(e, _errorMessageTemplate, e.Message, context.Request.Headers, context.Request.Body);
                await HandleExceptionAsync(context, e);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var resultError = new ErrorResult();
            resultError.AddError(exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(resultError, JsonSettings));
        }

        private static Task HandleExceptionAsync(HttpContext context, RequestException exception)
        {
            var resultError = new ErrorResult();
            foreach (var error in exception.ErrorList)
                resultError.AddError(error);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(resultError, JsonSettings));
        }

        private static Task HandleExceptionAsync(HttpContext context, AuthenticationException exception)
        {
            var resultError = new ErrorResult();
            foreach (var error in exception.ErrorList)
                resultError.AddError(error);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(resultError, JsonSettings));
        }

        private static Task HandleExceptionAsync(HttpContext context, PermissionException exception)
        {
            var resultError = new ErrorResult();
            foreach (var error in exception.ErrorList)
                resultError.AddError(error);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(resultError, JsonSettings));
        }

        private static Task HandleExceptionAsync(HttpContext context, ConflictException exception)
        {
            var resultError = new ErrorResult();
            foreach (var error in exception.ErrorList)
                resultError.AddError(error);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(resultError, JsonSettings));
        }

        private static Task HandleExceptionAsync(HttpContext context, BusinessException exception)
        {
            var resultError = new ErrorResult();
            foreach (var error in exception.ErrorList)
                resultError.AddError(error);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(resultError, JsonSettings));
        }

        private static Task HandleExceptionAsync(HttpContext context, RepositoryException exception)
        {
            var resultError = new ErrorResult();
            foreach (var error in exception.ErrorList)
                resultError.AddError(error);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(resultError, JsonSettings));
        }

        private static Task HandleExceptionAsync(HttpContext context, NotFoundException exception)
        {
            var resultError = new ErrorResult();
            foreach (var error in exception.ErrorList)
                resultError.AddError(error);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(resultError, JsonSettings));
        }
    }
}