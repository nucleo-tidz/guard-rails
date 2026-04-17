using application.Services.Interfaces;

using infrastructure.Agents;
using infrastructure.Agents.Adaptors;
using infrastructure.Agents.Guardrails;
using infrastructure.Agents.Services;
using infrastructure.Options;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        =>
            services.AddScoped<IEmbedService, EmbedService>()
            .AddScoped<ITextSearchAdapter, TextSearchAdapter>()
            .AddScoped<INucleotidzAgent, NucleotidzAgent>();



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

        public static IServiceCollection AddGuardRail(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(ContentSafetyOptions.SectionName);

            var options = Microsoft.Extensions.Configuration.ConfigurationBinder.Get<ContentSafetyOptions>(section)
                ?? throw new InvalidOperationException($"Missing configuration section: {ContentSafetyOptions.SectionName}");

            services.Configure<ContentSafetyOptions>(s =>
            {
                s.Uri = options.Uri;
                s.Key = options.Key;
            });

            services.AddHttpClient<IGroundnessDetector, GroundnessDetector>()
              .ConfigureHttpClient((serviceProvider, client) =>
              {
                  client.BaseAddress = new Uri(options.Uri);
                  client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", options.Key);
              });
            return services;
        }
    }
}
