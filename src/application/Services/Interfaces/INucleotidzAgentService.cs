namespace application.Services.Interfaces
{
    using model;

    public interface INucleotidzAgentService
    {
        Task<string> Start( string message);
        Task OnBoard(TokenLimitModel tokenLimitModel);
    }
}
