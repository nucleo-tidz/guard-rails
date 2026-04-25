namespace application.Services.Interfaces
{
    using Microsoft.Extensions.AI;
    using model.Enums;

    public interface IQueryIntentClassifier
    {
        Task<QueryIntent> ClassifyAsync(string userMessage, IEnumerable<ChatMessage>? recentHistory = null, CancellationToken ct = default);
    }
}
