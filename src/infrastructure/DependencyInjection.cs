
namespace infrastructure
{
    using System;

    using application.Services.Interfaces;

    using infrastructure.Agents;
    using infrastructure.Agents.Adaptors;
    using infrastructure.Agents.Guardrails;
    using infrastructure.Agents.HistoryProvider;
    using infrastructure.Agents.Midllewares;
    using infrastructure.Agents.Plugin;
    using infrastructure.Agents.Services;
    using infrastructure.Options;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using model;

    using OpenAI.Chat;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        =>
            services.AddScoped<IEmbedService, EmbedService>()
            .AddScoped<ITextSearchAdapter, TextSearchAdapter>()
            .AddScoped<INucleotidzAgent, NucleotidzAgent>()
            .AddScoped<IShipmentPlugin, ShipmentPlugin>()
            .AddScoped<IGuardRailMiddleware, GuardRailMiddleware>().AddScoped<IQueryIntentClassifier, QueryIntentClassifier>()
            .AddAIAgent(configuration);
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

            services.AddHttpClient<IJailBreakDetector, JailBreakDetector>()
             .ConfigureHttpClient((serviceProvider, client) =>
             {
                 client.BaseAddress = new Uri(options.Uri);
                 client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", options.Key);
             });
            services.AddHttpClient<IPersonalCaetgoryDetector, PersonalCaetgoryDetector>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                client.BaseAddress = new Uri(options.Uri);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", options.Key);
            });
            return services;
        }

        public static IServiceCollection AddAIAgent(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKeyedScoped<AIAgent>("nucleotidz", static (sp, key) =>
            {
                var plugin = sp.GetRequiredService<IShipmentPlugin>();
                var sharedContext = sp.GetRequiredService<ISharedContext>();
                var embeddingGenerator = sp.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
                return new ChatClientAgent
                (
                    chatClient: sp.GetRequiredService<IChatClient>(),
                    options: new ChatClientAgentOptions
                    {
                        ChatOptions = new ChatOptions
                        {
                            Instructions = """
                     You are a professional shipment assistant for shiptech company with expertise in freight and container logistics.
                     You help users retrieve accurate, up-to-date information about their shipment bookings.
                     
                     ## Capabilities
                     You can answer questions about a booking by looking up:
                     - Booking status (e.g. In Transit, Arrived, Pending)
                     - Total number of containers and their individual container numbers
                     - Total cargo weight in kilograms
                     - Port of origin and destination port
                     - Assigned vessel name and voyage number
                     - Estimated time of arrival (ETA)
                     - Provide information of shiptech company policy
                     
                     ## Behaviour
                     - Always ask for a booking ID before calling any tool, unless the user has already provided one.
                     - Call only the tools required to answer the user's specific question — do not retrieve unnecessary data.
                     - If a tool returns no data or an error, inform the user clearly and suggest they verify their booking ID.
                     - Never guess or fabricate shipment data. Only present information returned by your tools.
                     - If the user asks something outside your capabilities (e.g. modifying a booking), politely explain that you can only retrieve information and direct them to the appropriate team.
                     
                     ## Response style
                     - Be concise and professional.
                     - Present structured data (e.g. container lists) in a readable format.
                     """,
                            ToolMode = ChatToolMode.Auto,
                            Tools = [

                                    AIFunctionFactory.Create(plugin.GetTotalContainers),
                                    AIFunctionFactory.Create(plugin.GetBookingStatus),
                                    AIFunctionFactory.Create(plugin.GetTotalCargoWeight),
                                    AIFunctionFactory.Create(plugin.GetOriginPort),
                                    AIFunctionFactory.Create(plugin.GetDestinationPort),
                                    AIFunctionFactory.Create(plugin.GetEstimatedArrival),
                                    AIFunctionFactory.Create(plugin.GetVesselDetails),
                                    AIFunctionFactory.Create(plugin.GetContainerNumbers),
                                   ],
                        },
                        Description = "A Nucleotidz company assistant",
                        ChatHistoryProvider = new RedisChatHistoryProvider(summarizingChatReducer: new SummarizingChatReducer(sp.GetRequiredService<IChatClient>(), 2, 3)),
                        AIContextProviders = [new TextSearchProvider(sp.GetRequiredService<ITextSearchAdapter>().Search, new() {SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke, RecentMessageMemoryLimit = 5})
                        ],

                    }
                )
                .AsBuilder()
                .Use(sp.GetRequiredService<IGuardRailMiddleware>().JailBreakDetection, null)
                // .Use(sp.GetRequiredService<IGuardRailMiddleware>().PersonalCategoryDetection, null)
                //.Use(sp.GetRequiredService<IGuardRailMiddleware>().GroudnessDetection, null)
                .Build();

            });
            return services;
        }
    }
}
