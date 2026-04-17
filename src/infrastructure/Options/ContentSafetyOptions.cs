namespace infrastructure.Options
{
    public class ContentSafetyOptions
    {
        public const string SectionName = "ContentSafety";

        public string Uri { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }
}
