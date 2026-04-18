namespace application.Services.Interfaces
{
    public interface INucleotidzAgent
    {
        Task<string> Start( string message);
    }
}
