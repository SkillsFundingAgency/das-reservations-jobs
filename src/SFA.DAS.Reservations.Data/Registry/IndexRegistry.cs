using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Reservations.Data.Registry
{
    public class IndexRegistry : IIndexRegistry
    {
        public const string Name = "reservations-index-registry";

        private readonly IElasticClient _client;
        private List<IndexRegistryEntry> _indexRegistries;

        public string CurrentIndexName { get; private set; }

        public IndexRegistry(IElasticClient client)
        {
            _client = client;

            RefreshRegistry();
        }

        public async Task Add(string indexName)
        {
            var item = new IndexRegistryEntry
            {
                Id = Guid.NewGuid(),
                Name = indexName,
                DateCreated = DateTime.Now
            };

            var response = await _client.IndexAsync(new IndexRequest<IndexRegistryEntry>(item, Name));

            if (response.IsValid)
            {
                CurrentIndexName = indexName;
            }
        }

        public async Task DeleteOldIndices(uint daysOld)
        {
            var indicesToDelete = _indexRegistries.Where(x => 
                x.DateCreated <= DateTime.Now.AddDays(-daysOld)).ToArray();

            var response = await _client.DeleteManyAsync(indicesToDelete, Name);

            if (response.IsValid)
            {
                _indexRegistries = _indexRegistries.Except(indicesToDelete).ToList();

                if (!_indexRegistries.Any())
                {
                    CurrentIndexName = null;
                }
            }
        }

        private void RefreshRegistry()
        {
            var entries = _client.Search<IndexRegistryEntry>(descriptor => descriptor.Index(Name).MatchAll());
            _indexRegistries = entries?.Documents?.ToList() ?? new List<IndexRegistryEntry>();

            if (_indexRegistries.Any())
            {
                CurrentIndexName = _indexRegistries.OrderByDescending(r => r.DateCreated).First().Name;
            }
        }
    }
}
