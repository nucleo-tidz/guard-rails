namespace api.Filter
{
    using System.Threading.Tasks;

    using infrastructure.RateLimiters;

    using Microsoft.AspNetCore.Mvc.Filters;

    public class RateLimiterFilter(ILimiter fixedWindowLimiter) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.Request.Headers.TryGetValue("x-client", out var clientId);
            if (string.IsNullOrEmpty(clientId))
            {
                context.HttpContext.Response.StatusCode = 400;
                await context.HttpContext.Response.WriteAsync("Missing x-client header.");
                return;
            }
            if (!await fixedWindowLimiter.IsRequestAllowedAsync(clientId))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers.RetryAfter = "300"; 
                await context.HttpContext.Response.WriteAsync("Too many request Rate Limited");
                return;
            }
            await next();
        }
    }
}
