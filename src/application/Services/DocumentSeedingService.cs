namespace application.Services
{
    using application.Services.Interfaces;
  

    public class DocumentSeedingService : IDocumentSeedingService
    {
        private readonly IEmbedService embedService;

        public DocumentSeedingService(IEmbedService embedService)
        {
            this.embedService = embedService;
        }

        public async Task SeedDocumentAsync(string documentContent, string sourceFileName, string collectionName = "nucleotidz", int chunkSize = 200, int overlap = 50)
        {
            await embedService.SeedDataAsync(documentContent, sourceFileName, collectionName, chunkSize, overlap);
        }
    }
}
