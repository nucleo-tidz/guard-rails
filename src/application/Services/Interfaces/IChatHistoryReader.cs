namespace application.Services.Interfaces
{
    using Microsoft.Extensions.AI;

    public interface IChatHistoryReader
    {
        Task<IEnumerable<ChatMessage>> GetRecentMessagesAsync(int count, CancellationToken ct = default);
    }
}
