namespace application
{
    using Microsoft.Extensions.DependencyInjection;
    using application.Services;
    using application.Services.Interfaces;

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        => services.AddScoped<IDocumentSeedingService, DocumentSeedingService>()
            .AddScoped<ISharedContextService, SharedContextService>()
            .AddScoped<INucleotidzAgentService,NucleotidzAgentService>();


    }
}
