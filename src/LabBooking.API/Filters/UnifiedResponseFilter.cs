using LabBooking.Application.Common.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LabBooking.API.Filters;

public class UnifiedResponseFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            var resultType = objectResult.Value?.GetType();
            if (resultType != null && resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                await next();
                return;
            }

            var statusCode = objectResult.StatusCode ?? 200;

            string message;
            object? data = objectResult.Value;
            object? errors = null;

            if (statusCode >= 200 && statusCode < 300)
            {
                message = "Success";
            }
            else
            {
                if (objectResult.Value is string errorMessage)
                {
                    message = errorMessage;
                    data = null;
                }
                else
                {
                    message = "Error";
                }
            }

            var response = new ApiResponse<object>(
                statusCode,
                message,
                data,
                errors
            );

            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };
        }
        else if (context.Result is StatusCodeResult statusCodeResult || context.Result is EmptyResult)
        {
            var status = (context.Result as StatusCodeResult)?.StatusCode ?? 200;

            string msg = status switch
            {
                401 => "Unauthorized",
                403 => "Forbidden",
                >= 400 => "Error",
                _ => "Success"
            };

            var finalStatus = status == 204 ? 200 : status;

            var response = new ApiResponse<object>(
               finalStatus,
               msg,
               null
           );

            context.Result = new ObjectResult(response)
            {
                StatusCode = finalStatus
            };
        }

        await next();
    }
}

