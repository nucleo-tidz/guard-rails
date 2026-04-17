namespace api.RequestModels
{
    public class SeedDocumentRequest
    {
        public IFormFile File { get; set; } = null!;
        public string CollectionName { get; set; } = "nucleotidz";
        public int ChunkSize { get; set; } = 200;
        public int Overlap { get; set; } = 50;
    }
}
