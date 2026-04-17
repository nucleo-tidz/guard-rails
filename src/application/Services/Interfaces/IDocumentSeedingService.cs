namespace application.Services.Interfaces
{
    public interface IDocumentSeedingService
    {
        Task SeedDocumentAsync(string documentContent, string sourceFileName, string collectionName = "nucleotidz", int chunkSize = 200, int overlap = 50);
    }
}
