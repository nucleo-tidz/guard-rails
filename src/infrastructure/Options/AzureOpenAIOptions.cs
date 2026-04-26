namespace infrastructure.Options
{
    public class AzureOpenAIOptions
    {
        public const string SectionName = "AzureOpenAI";

        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ChatModelName { get; set; } = string.Empty;
        public string EmbeddingModelName { get; set; } = string.Empty;
        public string LightChatModelName { get; set; } = string.Empty;
    }
}
