namespace infrastructure.Agents.Adaptors
{
    using System;
    using System.Collections.Generic;

    using application.Dtos;
    using Microsoft.Agents.AI;

    internal interface ITextSearchAdapter
    {
        List<RagContext> _context { get; }
        Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchAdapter(string text, CancellationToken ct);
    }
}
