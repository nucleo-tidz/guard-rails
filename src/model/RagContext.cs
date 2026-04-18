namespace model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class RagContext
    {
        public string Text { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        public string SourceLink { get; set; } = string.Empty;
        public object? RawRepresentation;
    }
}
