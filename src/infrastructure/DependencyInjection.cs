using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using infrastructure.Options;

namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
           return services
        }
  
        public static IServiceCollection AddAI(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(AzureOpenAIOptions.SectionName);

            var options = Microsoft.Extensions.Configuration.ConfigurationBinder.Get<AzureOpenAIOptions>(section)
                ?? throw new InvalidOperationException($"Missing configuration section: {AzureOpenAIOptions.SectionName}");

            services.Configure<AzureOpenAIOptions>(s =>
            {
                s.Endpoint = options.Endpoint;
                s.ApiKey = options.ApiKey;
                s.ChatModelName = options.ChatModelName;
                s.EmbeddingModelName = options.EmbeddingModelName;
            });

            var client = new Azure.AI.OpenAI.AzureOpenAIClient(new Uri(options.Endpoint),
               new System.ClientModel.ApiKeyCredential(options.ApiKey));
            services.AddChatClient(client.GetChatClient(options.ChatModelName).AsIChatClient());
            services.AddEmbeddingGenerator<string, Embedding<float>>(client.GetEmbeddingClient(options.EmbeddingModelName).AsIEmbeddingGenerator());
            return services;
        }      
    }
}
