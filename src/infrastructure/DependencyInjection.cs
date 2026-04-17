using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;
using Azure.Identity;

using infrastructure.Agents.AzureAgents;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
        public static IServiceCollection AddAzureAIAgent(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(_ =>
            {
                var credential = new DefaultAzureCredential(
                new DefaultAzureCredentialOptions
                {
                    ExcludeVisualStudioCredential = true,
                    ExcludeEnvironmentCredential = true,
                    ExcludeManagedIdentityCredential = true,
                    ExcludeInteractiveBrowserCredential = false,
                    ExcludeAzureCliCredential = false,
                    ExcludeAzureDeveloperCliCredential = true,
                    ExcludeAzurePowerShellCredential = true,
                    ExcludeSharedTokenCacheCredential = true,
                    ExcludeVisualStudioCodeCredential = true,
                    ExcludeWorkloadIdentityCredential = true,

                });
                var projectClient = new AIProjectClient(new Uri(configuration["AgentProjectEndpoint"]), credential);
                return projectClient.GetPersistentAgentsClient();
            });
            services.AddTransient<INucleotidzAgent, NucleotidzAgent>();
            return services;
        }
        public static IServiceCollection AddSemanticKernel(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddTransient<Kernel>(serviceProvider =>
            {
                IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
                kernelBuilder.Services.AddAzureOpenAIChatCompletion("o4-mini",
                  configuration["o4-mini-url"],
                  configuration["o4-mini-key"],
                   "o4-mini",
                   "o4-mini");
                return kernelBuilder.Build();
            });
        }      
    }
}
