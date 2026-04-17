namespace infrastructure.Agents.Adaptors
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using infrastructure.Agents.Model;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;
    using Microsoft.SemanticKernel.Connectors.Redis;

    using StackExchange.Redis;

    internal class TextSearchAdapter : ITextSearchAdapter
    {
        private readonly RedisVectorStore vectorStore;
        public TextSearchAdapter(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            vectorStore = new RedisVectorStore(redis.GetDatabase(), new RedisVectorStoreOptions { EmbeddingGenerator = embeddingGenerator });
        }
        public async Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchAdapter(string text, CancellationToken ct)
        {
            var documentCollection = vectorStore.GetCollection<Guid, VectorModel>("nucleotidz");
            List<TextSearchProvider.TextSearchResult> results = [];
            await foreach (var result in documentCollection.SearchAsync(text, 5, cancellationToken: ct))
            {
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
