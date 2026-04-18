namespace infrastructure.Agents
{
    using System.Threading;
    using System.Threading.Tasks;

    using application.Services.Interfaces;

    using Microsoft.Extensions.AI;

    internal class QueryIntentClassifier(IChatClient chatClient) : IQueryIntentClassifier
    {
        public QueryIntent queryIntent { get; private set; }       
        public async Task ClassifyAsync(string userMessage, CancellationToken ct = default)
        {
            var classificationPrompt = $"""
                Classify the user's message into strictly ONE of these categories:
                - BookingQuery: Questions about specific shipments, bookings, tracking, container numbers, status
                - CompanyPolicy: Questions about company policies, procedures, terms, rates, insurance
                - Mixed: Questions involving both booking details and company information  
                
                Respond with ONLY one category name: BookingQuery, CompanyPolicy, or Mixed
                """;

            try
            {
                ChatResponse<QueryIntent> response =
                     await chatClient.GetResponseAsync<QueryIntent>(
                         [
                          new Microsoft.Extensions.AI.ChatMessage(ChatRole.System, classificationPrompt),
                          new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, userMessage)
                     ], cancellationToken: ct);

                queryIntent = response.Result;

            }
            catch
            {
                // Fallback to safe default on LLM error
                queryIntent = QueryIntent.General;
            }
        }
    }
}
