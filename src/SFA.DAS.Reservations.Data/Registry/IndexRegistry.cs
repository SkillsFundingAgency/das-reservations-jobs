using System;
using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Reservations.Data.Registry
{
    public class IndexRegistry : IIndexRegistry
    {
        public const string Name = "reservations-index-registry";

        private readonly IElasticClient _client;

        public string CurrentIndexName { get; private set; }

        public IndexRegistry(IElasticClient client)
        {
            _client = client;

            CreateIndex();
        }

        public async Task Add(string indexName)
        {
            var item = new IndexRegistryEntry
            {
                Name = indexName,
                DateCreated = DateTime.Now
            };

            await _client.IndexAsync(new IndexRequest<IndexRegistryEntry>(item, Name));

            CurrentIndexName = indexName;
        }

        private void CreateIndex()
        {
            if (!_client.Indices?.Exists(new IndexExistsRequest(Name)).Exists ?? false)
            {
                _client.Indices.Create(new CreateIndexRequest(Name));
            }
        }
    }
}
