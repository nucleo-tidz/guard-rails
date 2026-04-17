namespace infrastructure.Agents.Adaptors
{
    using System.Collections.Generic;

    using Microsoft.Agents.AI;

    internal interface ITextSearchAdapter
    {
        Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchAdapter(string text, CancellationToken ct);
    }
}
