using application.Services.Interfaces;

using infrastructure.Agents;
using infrastructure.Agents.Adaptors;
using infrastructure.Agents.Guardrails;
using infrastructure.Agents.HistoryProvider;
using infrastructure.Agents.Services;
using infrastructure.Options;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenAI.Chat;

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

        public static IServiceCollection AddAIAgent(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKeyedSingleton<AIAgent>("nucleotidz",(sp) => new ChatClientAgent(
            chatClient: sp.GetService<IChatClient>(),
            options: new ChatClientAgentOptions
            {
                ChatOptions = new ChatOptions()
                {
                    Instructions = "You are a shiptech Agent providing user information on shiptech company, you can also guide use which container is best to book based on user's need",
                    ToolMode = ChatToolMode.Auto,
                },
                Description = "A shiptech company assistant",
                ChatHistoryProvider = new RedisChatHistoryProvider(summarizingChatReducer: new SummarizingChatReducer(sp.GetService<IChatClient>(), 2, 3)),
            }));
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
