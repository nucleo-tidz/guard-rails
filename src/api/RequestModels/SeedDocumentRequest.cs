namespace api.RequestModels
{
    public class SeedDocumentRequest
    {
        public IFormFile File { get; set; } = null!;
        public int ChunkSize { get; set; } = 200;
        public int Overlap { get; set; } = 50;
    }
}
