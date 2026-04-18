namespace infrastructure.Agents.Adaptors
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using infrastructure.Agents.Model;
    using application.Dtos;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;
    using Microsoft.SemanticKernel.Connectors.Redis;

    using StackExchange.Redis;

    internal class TextSearchAdapter : ITextSearchAdapter
    {
        private readonly RedisVectorStore vectorStore;
        public List<RagContext> _context { get; private set; } = new();

        public TextSearchAdapter(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            vectorStore = new RedisVectorStore(redis.GetDatabase(), new RedisVectorStoreOptions { EmbeddingGenerator = embeddingGenerator });
        }

        public async Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchAdapter(string text, CancellationToken ct)
        {
            var documentCollection = vectorStore.GetCollection<Guid, VectorModel>("nucleotidz");
            List<TextSearchProvider.TextSearchResult> results = [];

            if (_context.Any())
            {
                foreach (var context in _context)
                {
                    results.Add(new TextSearchProvider.TextSearchResult
                    {
                        SourceName = context.SourceName,
                        SourceLink = context.SourceLink,
                        Text = context.Text,
                        RawRepresentation = context.RawRepresentation
                    });
                }
                return results;
            }

            await foreach (var result in documentCollection.SearchAsync(text, 5, cancellationToken: ct))
            {
                _context.Add(new RagContext { Text = result.Record.Text ?? string.Empty, RawRepresentation = result, SourceLink = result.Record.SourceLink ,SourceName= result.Record.SourceName });
                results.Add(new TextSearchProvider.TextSearchResult
                {
                    SourceName = result.Record.SourceName,
                    SourceLink = result.Record.SourceLink,
                    Text = result.Record.Text ?? string.Empty,
                    RawRepresentation = result
                });
            }

            return results;
        }
    }
}
