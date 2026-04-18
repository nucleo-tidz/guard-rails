namespace application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using application.Services.Interfaces;

    using model;

    internal class SharedContextService(ISharedContext sharedContext, IQueryIntentClassifier queryIntentClassifier, ITextSearchAdapter textSearchAdapter) : ISharedContextService
    {
        public async Task Build(string user, string threadId, string query)
        {
            sharedContext.User = user;
            sharedContext.ThreadId = threadId;
            sharedContext.queryIntent = await queryIntentClassifier.ClassifyAsync(query, CancellationToken.None);
            sharedContext.ragContexts.Clear();
            if (sharedContext.queryIntent == model.Enums.QueryIntent.CompanyPolicy || sharedContext.queryIntent == model.Enums.QueryIntent.Mixed)
            {
                await textSearchAdapter.Search(query, CancellationToken.None);
            }
        }
    }
}
