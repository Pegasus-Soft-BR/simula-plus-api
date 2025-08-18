using Domain.Common;
using Domain.Exceptions;
using Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MockExams.Api.Services;
using Rollbar;
using System;
using System.Threading.Tasks;

namespace MockExams.Api.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (BizException ex)
        {
            var result = new Result();
            result.Messages.Add(ex.Message);
            var jsonResponse = JsonHelper.ToJson(result);

            httpContext.Response.Clear();
            httpContext.Response.StatusCode = (int)ex.ErrorType;
            httpContext.Response.Headers.Add("Content-Type", "application/json");
            await httpContext.Response.WriteAsync(jsonResponse);
        }
        catch (Exception ex)
        {
            if (RollbarConfigurator.IsActive)
            {
                SendErrorToRollbar(ex);
            }

            var result = new Result();
            result.Messages.Add(ex.ToString());

            // detalhes do erro real pra facilitar o desenvolvimento.
            if (ex is AggregateException)
                result.Messages.Add(ex.InnerException.ToString());

            var jsonResponse = JsonHelper.ToJson(result);

            httpContext.Response.Clear();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.Headers.Add("Content-Type", "application/json");
            await httpContext.Response.WriteAsync(jsonResponse);
        }
    }

    private void SendErrorToRollbar(Exception ex)
    {
        object error = new
        {
            Message = ex.Message,
            StackTrace = ex.StackTrace,
            Source = ex.Source
        };

        RollbarLocator.RollbarInstance.Error(error);
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<ExceptionHandlerMiddleware>();
}