namespace infrastructure.Agents.HistoryProvider
{
    using System;
    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;
    using Microsoft.SemanticKernel.Connectors.Redis;
    using StackExchange.Redis;

    /// <summary>
    /// Implementation of ChatHistoryMemoryProvider builder
    /// Simplifies the creation of ChatHistoryMemoryProvider with sensible defaults
    /// </summary>
    internal class MemoryProviderBuilder 
    {
        private readonly RedisVectorStore _vectorStore;
        private string _collectionName = "memory";
        private int _vectorDimensions = 3072;
        private string _userId = string.Empty;
        private string _sessionId = string.Empty;

        public MemoryProviderBuilder(            
            IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            var vectorStoreOptions = new RedisVectorStoreOptions
            {
                EmbeddingGenerator = embeddingGenerator
            };
            _vectorStore = new RedisVectorStore(redis.GetDatabase(), vectorStoreOptions);
        }

        public MemoryProviderBuilder WithCollectionName(string collectionName)
        {
            _collectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
            return this;
        }

        public MemoryProviderBuilder WithVectorDimensions(int dimensions)
        {
            if (dimensions <= 0)
                throw new ArgumentException("Dimensions must be greater than 0", nameof(dimensions));

            _vectorDimensions = dimensions;
            return this;
        }

        public MemoryProviderBuilder WithStorageScope(string userId, string sessionId)
        {
            _userId = userId ?? throw new ArgumentNullException(nameof(userId));
            _sessionId = sessionId ?? throw new ArgumentNullException(nameof(sessionId));
            return this;
        }

        public ChatHistoryMemoryProvider Build()
        {
            if (string.IsNullOrWhiteSpace(_userId))
                throw new InvalidOperationException("Storage scope must be set before building");

            return new ChatHistoryMemoryProvider(
                _vectorStore,
                collectionName: _collectionName,
                vectorDimensions: _vectorDimensions,
                session => new ChatHistoryMemoryProvider.State(
                    storageScope: new() { UserId = _userId, SessionId = _sessionId },
                    searchScope: new() { UserId = _userId }));
        }
    }
}
