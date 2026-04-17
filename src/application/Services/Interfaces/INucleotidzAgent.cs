namespace application.Services.Interfaces
{
    using application.Dtos;

    public interface INucleotidzAgent
    {
        Task<string> Start(string conversationId, string UserId, string message);
    }
}
