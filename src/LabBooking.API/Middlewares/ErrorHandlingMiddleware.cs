using FluentValidation;

namespace LabBooking.API.Middlewares;

public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (ValidationException validationException)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/problem+json";

            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

            var problemDetails = new ValidationProblemDetails(errors)
            {
                Status = 400,
                Title = "One or more validation errors occurred.",
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (BadRequestException badRequest)
        {
            logger.LogWarning(badRequest.Message);
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = 400,
                Title = "Bad Request",
                Detail = badRequest.Message,
                Instance = context.Request.Path
            };
            await context.Response.WriteAsJsonAsync(problemDetails);

        }
        catch (UnauthorizedAccessException unauth)
        {
            logger.LogWarning(unauth.Message);
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = 401,
                Title = "Unauthorized",
                Detail = unauth.Message,
                Instance = context.Request.Path
            };
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (ForbidException forbid)
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = 403,
                Title = "Forbidden",
                Detail = forbid.Message,
                Instance = context.Request.Path
            };
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (NotFoundException notFound)
        {
            logger.LogWarning(notFound.Message);
            context.Response.StatusCode = 404;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = 404,
                Title = "Not Found",
                Detail = notFound.Message,
                Instance = context.Request.Path
            };
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = 500,
                Title = "Internal Server Error",
                Detail = "Something went wrong. Please try again later.",
                Instance = context.Request.Path
            };
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
