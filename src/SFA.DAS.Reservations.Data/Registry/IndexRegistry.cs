using System;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Data.ElasticSearch;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Data.Registry
{
    public class IndexRegistry : IIndexRegistry
    {
        public string Name { get; }

        private readonly IElasticLowLevelClient _client;
        private readonly IElasticSearchQueries _queries;
        private readonly ReservationJobsEnvironment _environment;

        public string CurrentIndexName { get; private set; }

        public IndexRegistry(IElasticLowLevelClient client, IElasticSearchQueries queries, ReservationJobsEnvironment environment)
        {
            _client = client;
            _queries = queries;
            _environment = environment;
            Name = environment.EnvironmentName + queries.ReservationIndexLookupName;
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
                .Where(x => x.DateCreated > DateTime.Now.AddDays(-daysOld))
                .ToList();

            var oldIndices = _client.Indices
                .Get(new GetIndexRequest(Indices.All))
                .Indices
                .Where(c=>c.Key.Name.StartsWith($"{_environment.EnvironmentName}-reservations"))
                .Where(c =>c.Key.Name != Name)
                .ToList();

            foreach (var entry in oldIndices)
            {
                if(registriesToKeep.Select(c => c.Name).All(c => c != entry.Key) 
                   && entry.Key != Name)
                {
                    await _client.Indices.DeleteAsync(entry.Key.Name);    
                }
                
            }
        }

        private void SetCurrentIndexName()
        {
            var data = PostData.String(_queries.LastIndexSearchQuery);
            
            var response =  _client.Search<StringResponse>(
                _environment.EnvironmentName + _queries.ReservationIndexLookupName, data);

            if (response?.Body == null)
            {
                return;
            }
            
            var elasticResponse = JsonConvert.DeserializeObject<ElasticResponse<IndexRegistryEntry>>(response.Body);

            if (elasticResponse?.Items != null && elasticResponse.Items.Any())
            {
                CurrentIndexName = elasticResponse.Items.First().Name;
            }
        }
    }
}
