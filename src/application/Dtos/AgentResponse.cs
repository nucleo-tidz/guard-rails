namespace application.Dtos
{
    using System.Collections.Generic;

    public class RagContext
    {
        public string Text { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        public string SourceLink { get; set; } = string.Empty;
        public object? RawRepresentation;
    }

  
}
