namespace application.Services.Interfaces
{
    using System;
    using System.Collections.Generic;

 
    using Microsoft.Agents.AI;

    public interface ITextSearchAdapter
    {
        Task<IEnumerable<TextSearchProvider.TextSearchResult>> Search(string text, CancellationToken ct);
    }
}
