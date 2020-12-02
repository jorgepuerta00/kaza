using App.Common.Exceptions;
using App.Common.SeedWork;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace App.Common.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch(AppException e)
            {
                await WriteErrorResponse(httpContext, e);
            }
            catch (Exception e)
            {
                await WriteErrorResponse(httpContext, e);
            }
        }

        private async Task WriteErrorResponse(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = "application/json";            

            ActionResponse responseObj = null;
            if (exception is AppException)
            {
                responseObj = (exception as AppException).ActionResponse;
                httpContext.Response.StatusCode = (int)responseObj.StatusCode;
            }
            else // Generic exception
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                responseObj = new ActionResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Title = "An unexpected error has occurred, please try again later"
                };                
            }

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(responseObj));
        }
    }
}
