using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Data.Registry
{
    public class IndexRegistry : IIndexRegistry
    {
        private string Name { get; }

        private readonly IElasticLowLevelClientWrapper _client;
        private readonly IElasticSearchQueries _queries;

        public string CurrentIndexName { get; private set; }

        public IndexRegistry(IElasticLowLevelClientWrapper client, IElasticSearchQueries queries, ReservationJobsEnvironment environment)
        {
            _client = client;
            _queries = queries;
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
            var data = JsonConvert.SerializeObject(item);
            
            var response = await _client.Create(Name, item.Id.ToString(), data);
            
            if (response.result.ToLower().Equals("created"))
            {
                CurrentIndexName = indexName;
            }
        }

        public async Task DeleteOldIndices(uint daysOld)
        {   
            var query = _queries.LastIndexSearchQuery.Replace(@"""{size}""", "100");
            
            var response =  await _client.Search(Name, query);

            if (response?.Body == null)
            {
                return;
            }
            
            var elasticResponse = JsonConvert.DeserializeObject<ElasticResponse<IndexRegistryEntry>>(response.Body);

            var registriesToDelete = elasticResponse.Items
                .Where(x => x.DateCreated <= DateTime.Now.AddDays(-daysOld))
                .ToList();
            
            
            foreach (var registryEntry in registriesToDelete)
            {
                await _client.DeleteDocument(Name, registryEntry.Id.ToString());

                await _client.DeleteIndices(registryEntry.Name);
            }
            
        }

        private void SetCurrentIndexName()
        {
            var query = _queries.LastIndexSearchQuery.Replace(@"""{size}""", "1");
            
            var response =  _client.Search(Name, query).Result;

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
