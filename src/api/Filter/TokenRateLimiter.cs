namespace api.Filter
{
    using System.Threading.Tasks;

    using application.Services.Interfaces;

    using infrastructure.Agents.TokenManager;

    using Microsoft.AspNetCore.Mvc.Filters;

    public class TokenRateLimiter(ITokenLimiter tokenLimiter, IOnBoardRepository onBoardRepository) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var routeData = context.RouteData.Values;

            var userId = routeData.ContainsKey("username")
                ? routeData["username"]?.ToString()
                : null;

            bool Isonboarderd = await onBoardRepository.IsOnboarded(userId);
            if (!Isonboarderd)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                await context.HttpContext.Response.WriteAsync("User not onboarded f");
                return;
            }

            bool Isallowed = await tokenLimiter.IsAllowed();
            if (!Isallowed)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.WriteAsync("Token Limit Exhausted");
                return;
            }
            await next();
        }
    }
}
