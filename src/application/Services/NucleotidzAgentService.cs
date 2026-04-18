namespace application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using application.Services.Interfaces;

    using model;

    public class NucleotidzAgentService(ISharedContextService sharedContext,INucleotidzAgent nucleotidzAgent) : INucleotidzAgentService
    {
        public async Task<string> Start(string conversationId, string UserId, string message)
        {
           await sharedContext.Build(UserId, conversationId, message);
           return await nucleotidzAgent.Start(message);
        }
    }
}
