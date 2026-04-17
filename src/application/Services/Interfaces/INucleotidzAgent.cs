namespace application.Services.Interfaces
{
    public interface INucleotidzAgent
    {
        Task<string> Start(string conversationId, string UserId, string message);
    }
}
