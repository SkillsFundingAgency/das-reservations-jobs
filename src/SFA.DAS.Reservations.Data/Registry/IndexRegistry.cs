using System;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Data.Registry
{
    public class IndexRegistry : IIndexRegistry
    {
        public string Name { get; }

        private readonly IElasticClient _client;
        
        public string CurrentIndexName { get; private set; }

        public IndexRegistry(IElasticClient client, ReservationJobsEnvironment environment)
        {
            _client = client;
            Name = $"{environment.EnvironmentName}-reservations-index-registry";
            SetCurrentIndexName();
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
            var registryEntries = _client.Search<IndexRegistryEntry>(s => s
                .Index(Name)
                .From(0).Size(100));
            
            var registriesToDelete = registryEntries.Documents
                .Where(x => x.DateCreated <= DateTime.Now.AddDays(-daysOld))
                .ToList();

            if (!registriesToDelete.Any()) return;

            await _client.DeleteManyAsync(registriesToDelete, Name);

            var registriesToKeep = registryEntries.Documents
                .Where(x => x.DateCreated > DateTime.Now.AddDays(-daysOld));

            var oldIndices = _client.Indices
                .Get(new GetIndexRequest(Indices.All))
                .Indices
                .Where(pair => registriesToKeep.All(entry => entry.Name != pair.Key.Name));

            foreach (var entry in oldIndices)
            {
                await _client.Indices.DeleteAsync(entry.Key.Name);
            }
        }

        private void SetCurrentIndexName()
        {
            var entries = _client.Search<IndexRegistryEntry>(s => s
                .Index(Name)
                .From(0)
                .Size(1)
                .Sort(x => x.Descending(a => a.DateCreated)));
            
            if (entries.Documents.Any())
            {
                CurrentIndexName = entries.Documents.First().Name;
            }
        }
    }
}
