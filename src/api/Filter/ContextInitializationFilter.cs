namespace api.Filter
{
    using application.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ContextInitializationFilter : IAsyncActionFilter
    {
        private readonly ISharedContextService _sharedContext;

        public ContextInitializationFilter(ISharedContextService sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var routeData = context.RouteData.Values;

            var userId = routeData.ContainsKey("username")
                ? routeData["username"]?.ToString()
                : null;

            var threadId = routeData.ContainsKey("threadId")
                ? routeData["threadId"]?.ToString()
                : null;

            if (!string.IsNullOrEmpty(userId))
            {
                await _sharedContext.Build(userId, threadId);
            }

            await next();
        }
    }
}
