namespace LabBooking.API.Middlewares;

public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (BadRequestException badRequest)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(badRequest.Message);

            logger.LogWarning(badRequest.Message);
        }
        catch (UnauthorizedAccessException unauth)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(unauth.Message);

            logger.LogWarning(unauth.Message);
        }
        catch (ForbidException forbid)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync(forbid.Message);
        }
        catch (NotFoundException notFound)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(notFound.Message);

            logger.LogWarning(notFound.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Something went wrong");
        }
    }
}
