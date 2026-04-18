namespace infrastructure.Agents.Adaptors
{
    using System;
    using System.Collections.Generic;

    using application.Services.Interfaces;

    using infrastructure.Agents.Model;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;
    using Microsoft.SemanticKernel.Connectors.Redis;

    using model;
    using model.Enums;

    using StackExchange.Redis;

    internal class TextSearchAdapter : ITextSearchAdapter
    {
        private readonly RedisVectorStore vectorStore;
        private readonly ISharedContext _sharedContext;
        IQueryIntentClassifier _queryIntentClassifier;

        public TextSearchAdapter(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, ISharedContext sharedContext)
        {
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            vectorStore = new RedisVectorStore(redis.GetDatabase(), new RedisVectorStoreOptions { EmbeddingGenerator = embeddingGenerator });
            _sharedContext = sharedContext;
  
        }

        public async Task<IEnumerable<TextSearchProvider.TextSearchResult>> Search(string text, CancellationToken ct)
        {
            if (_sharedContext.queryIntent != QueryIntent.CompanyPolicy && _sharedContext.queryIntent != QueryIntent.Mixed)
            {
                return new List<TextSearchProvider.TextSearchResult>();
            }
            var documentCollection = vectorStore.GetCollection<Guid, VectorModel>("nucleotidz");
            List<TextSearchProvider.TextSearchResult> results = [];

            if (_sharedContext.ragContexts.Any())
            {
                foreach (var context in _sharedContext.ragContexts)
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
                _sharedContext.ragContexts.Add(new RagContext { Text = result.Record.Text ?? string.Empty, RawRepresentation = result, SourceLink = result.Record.SourceLink, SourceName = result.Record.SourceName });
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
