namespace infrastructure.Agents
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using application.Services.Interfaces;

    using Microsoft.Extensions.AI;

    using model.Enums;

    internal class QueryIntentClassifier(IChatClient chatClient) : IQueryIntentClassifier
    {
        public async Task<QueryIntent> ClassifyAsync(string userMessage, IEnumerable<ChatMessage>? recentHistory = null, CancellationToken ct = default)
        {
            const string classificationPrompt = """
                Classify the user's LATEST message into strictly ONE of these categories:
                - BookingQuery: Questions about specific shipments, bookings, tracking, container numbers, status
                - CompanyPolicy: Questions about company policies, procedures, terms, rates, insurance
                - Mixed: Questions involving both booking details and company information
                - General: Cannot be determined from context

                Use the conversation history (if provided) to resolve ambiguous references such as
                "What about its weight?" or "And the policy for that?".
                Respond with ONLY one category name: BookingQuery, CompanyPolicy, Mixed, or General
                """;

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, classificationPrompt)
            };

            if (recentHistory is not null)
                messages.AddRange(recentHistory);

            messages.Add(new ChatMessage(ChatRole.User, userMessage));

            try
            {
                ChatResponse<QueryIntent> response = await chatClient.GetResponseAsync<QueryIntent>(messages, cancellationToken: ct);
                return response.Result;
            }
            catch
            {
                return QueryIntent.General;
            }
        }
    }
}
