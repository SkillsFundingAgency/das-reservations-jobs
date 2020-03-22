using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Data.Registry
{
    public class ElasticLowLevelClientWrapper : IElasticLowLevelClientWrapper
    {
        private readonly IElasticLowLevelClient _client;

        public ElasticLowLevelClientWrapper (IElasticLowLevelClient client)
        {
            _client = client;
        }
        
        public async Task<StringResponse> Search(string name, string query)
        {
            var data = PostData.String(query);

            var searchAsync = await _client.SearchAsync<StringResponse>(name, data);
            return searchAsync;
        }

        public async Task<ElasticSearchResponse> Create(string name, string id, string item)
        {
            var data = PostData.String(JsonConvert.SerializeObject(item));
            
            var response = await _client.CreateAsync<ElasticSearchResponse>(name, id, data);

            return response;
        }

        public async Task DeleteDocument(string name, string id)
        {
            await _client.DeleteAsync<ElasticSearchResponse>(name, id);
        }

        public async Task DeleteIndices(string name)
        {
            await _client.Indices.DeleteAsync<ElasticSearchResponse>(name);
        }
        
    }
}