namespace application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using application.Services.Interfaces;

    using model;

    internal class SharedContextService(ISharedContext sharedContext) : ISharedContextService
    {
        public async Task Build(string user, string threadId)
        {
            sharedContext.User = user;
            sharedContext.ThreadId = threadId;
        }
    }
}
