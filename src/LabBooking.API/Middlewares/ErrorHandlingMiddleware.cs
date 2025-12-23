using FluentValidation;
using LabBooking.Application.Common.Wrappers;
using System.Text.Json;

namespace LabBooking.API.Middlewares;

public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context); // 1. Cho request đi qua các tầng khác (Auth, Controller...)

            // 2. Sau khi request quay về, kiểm tra xem có bị hệ thống chặn 401/403 không
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                await WriteErrorResponse(context, 401, "Bạn cần đăng nhập để thực hiện chức năng này.");
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                await WriteErrorResponse(context, 403, "Bạn không đủ quyền hạn để truy cập tài nguyên này.");
            }
        }
        catch (Exception ex)
        {
            // 3. Nếu có lỗi Exception văng ra từ bên trong (Logic cũ của bạn)
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        // Kiểm tra nếu response chưa bắt đầu gửi về client thì mới viết đè được
        if (!context.Response.HasStarted)
        {
            context.Response.ContentType = "application/json";

            var responseModel = new ApiResponse<object>(statusCode, message);

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(responseModel, jsonOptions);

            await context.Response.WriteAsync(json);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var responseModel = new ApiResponse<object>(500, "Internal Server Error");

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                responseModel.StatusCode = 400;
                responseModel.Message = "Validation Failed";
                // Gom lỗi validation vào Data
                var errors = validationException.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
                responseModel.Errors = errors;
                responseModel.Data = null;
                break;

            case BadRequestException badRequest:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                responseModel.StatusCode = 400;
                responseModel.Message = badRequest.Message;
                logger.LogWarning(badRequest.Message);
                break;

            case NotFoundException notFound:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                responseModel.StatusCode = 404;
                responseModel.Message = notFound.Message;
                logger.LogWarning(notFound.Message);
                break;

            case ForbiddenAccessException forbid:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                responseModel.StatusCode = 403;
                responseModel.Message = forbid.Message;
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                responseModel.StatusCode = 401;
                responseModel.Message = exception.Message;
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                responseModel.StatusCode = 500;
                responseModel.Message = "Something went wrong. Please try again later.";
                logger.LogError(exception, exception.Message);
                break;
        }

        // Serialize object thành JSON
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(responseModel, jsonOptions);

        await context.Response.WriteAsync(json);
    }
}
