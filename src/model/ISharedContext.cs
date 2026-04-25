namespace model
{
    using System.Collections.Generic;

    using model.Enums;

    public interface ISharedContext
    {
        List<RagContext> ragContexts { get; }
        QueryIntent queryIntent { get; set; }
        string User { get; set; }
        string ThreadId { get; set; }
        public string SessionKey => $"session:{User}:{ThreadId}";
        public string StateKey => $"chat:history:{User}:{ThreadId}";

    }
}
