namespace infrastructure.Agents.Services
{
    using System;
    using System.Collections.Generic;

    using application.Services.Interfaces;

    using infrastructure.Agents.Model;

    using Microsoft.Extensions.AI;
    using Microsoft.SemanticKernel.Connectors.Redis;

    using StackExchange.Redis;

    public class EmbedService: IEmbedService
    {
        private readonly RedisVectorStore vectorStore;


        public EmbedService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            vectorStore = new RedisVectorStore(redis.GetDatabase(), new RedisVectorStoreOptions { EmbeddingGenerator = embeddingGenerator });
        }

        public async Task SeedDataAsync(string documentContent, string sourceFileName, int chunkSize = 200, int overlap = 50)
        {
            if (string.IsNullOrWhiteSpace(documentContent))
            {
                throw new ArgumentException("Document content cannot be empty", nameof(documentContent));
            }

            if (string.IsNullOrWhiteSpace(sourceFileName))
            {
                throw new ArgumentException("Source file name cannot be empty", nameof(sourceFileName));
            }

            if (chunkSize <= 0)
            {
                throw new ArgumentException("Chunk size must be greater than 0", nameof(chunkSize));
            }

            var documentCollection = vectorStore.GetCollection<Guid, VectorModel>("nucleotidz");
            await documentCollection.EnsureCollectionDeletedAsync();
            await documentCollection.EnsureCollectionExistsAsync();

            var chunks = ChunkDocument(documentContent, sourceFileName, chunkSize, overlap);
            await documentCollection.UpsertAsync(chunks);
        }

        private List<VectorModel> ChunkDocument(string content, string sourceFileName, int chunkSize, int overlap)
        {
            var chunks = new List<VectorModel>();

            for (int i = 0; i < content.Length; i += chunkSize - overlap)
            {
                int endIndex = Math.Min(i + chunkSize, content.Length);
                var chunk = new VectorModel
                {
                    Key = Guid.NewGuid(),
                    SourceLink = sourceFileName,
                    SourceName = sourceFileName,
                    Text = content.Substring(i, endIndex - i)
                };
                chunks.Add(chunk);

                if (endIndex >= content.Length)
                {
                    break;
                }
            }

            return chunks;
        }
    }
}
