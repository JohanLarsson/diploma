using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Diploma.WebAPI.Infrastructure.Errors;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Diploma.WebAPI.Infrastructure
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex)
                    .ConfigureAwait(false);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var result = string.Empty;

            switch (exception)
            {
                case ValidationException ve:
                    context.Response.StatusCode = 422; // unprocessable entity

                    var errors = ve.Errors.ToDictionary(
                        pair => pair.PropertyName,
                        pair => pair.ErrorMessage);

                    result = JsonConvert.SerializeObject(
                        new
                        {
                            errors
                        });
                    
                    break;
                case RestException re:
                    context.Response.StatusCode = (int)re.Code;
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                result = JsonConvert.SerializeObject(
                    new
                    {
                        errors = exception.Message
                    });
            }

            // ReSharper disable once AsyncConverter.AsyncAwaitMayBeElidedHighlighting
            await context.Response.WriteAsync(result)
                .ConfigureAwait(false);
        }
    }
}
