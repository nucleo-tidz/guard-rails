namespace application.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface INucleotidzAgentService
    {
        Task<string> Start(string conversationId, string UserId, string message);
    }
}
