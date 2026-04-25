namespace application.Services.Interfaces
{
    using Microsoft.Extensions.AI;

    public interface IChatHistoryReader
    {
        Task<IEnumerable<ChatMessage>> GetRecentMessagesAsync(string userId, string conversationId, int count, CancellationToken ct = default);
    }
}
