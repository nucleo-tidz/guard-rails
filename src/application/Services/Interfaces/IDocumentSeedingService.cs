namespace application.Services.Interfaces
{
    public interface IDocumentSeedingService
    {
        Task SeedDocumentAsync(string documentContent, string sourceFileName, int chunkSize = 200, int overlap = 50);
    }
}
