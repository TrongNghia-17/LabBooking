using Microsoft.AspNetCore.Mvc.Filters;

namespace LabBooking.API.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 1. Lấy API Key từ Configuration (được set trong biến môi trường Render)
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var requiredApiKey = configuration["CRON_JOB_SECRET"];

        // 2. Kiểm tra Header
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("Missing API Key Header");
            return;
        }

        // 3. So sánh Key
        if (requiredApiKey == null || !requiredApiKey.Equals(extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("Invalid API Key");
            return;
        }

        await next();
    }
}
