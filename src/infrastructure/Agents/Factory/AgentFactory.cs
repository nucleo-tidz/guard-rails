using application.Services.Interfaces;
using infrastructure.Agents.HistoryProvider;
using infrastructure.Agents.Midllewares;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

using model;
using StackExchange.Redis;

namespace infrastructure.Agents.Factory
{
    internal class AgentFactory(
        IShipmentPlugin plugin,
        [FromKeyedServices("gpt")]IChatClient chatClient,
        [FromKeyedServices("mini")] IChatClient lightChatClient,
        IClassifierMiddleware classifierMiddleware,
        IGuardRailMiddleware guardRailMiddleware,
        ITextSearchAdapter textSearchAdapter,
        IConnectionMultiplexer redis,ISharedContext sharedContext) : IAgentFactory
    {
        public AIAgent Create()
        {
            return new ChatClientAgent
                 (
                     chatClient: chatClient,
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
                         Description = "A shiptech company assistant",
                         ChatHistoryProvider = new RedisChatHistoryProvider(redis, sharedContext, summarizingChatReducer: new SummarizingChatReducer(lightChatClient, 2, 3)),
                         AIContextProviders = [new TextSearchProvider(textSearchAdapter.Search, new() {SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke, RecentMessageMemoryLimit = 5})
                         ],

                     }
                 )
                 .AsBuilder()
                  .Use(classifierMiddleware.Classify, null)
                // .Use(guardRailMiddleware.JailBreakDetection, null)
                 //.Use(sp.GetRequiredService<IGuardRailMiddleware>().PersonalCategoryDetection, null)
                 .Use(guardRailMiddleware.GroudnessDetection, null)
                 .Build();
        }
    }

}
