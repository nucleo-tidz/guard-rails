namespace model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using model.Enums;

    public class SharedContext: ISharedContext
    {
        public List<RagContext> ragContexts { get; private set; } = new();
      
        public QueryIntent queryIntent { get; set; }

        public string User { get; set; }
        public string ThreadId { get; set; }
    }
}
