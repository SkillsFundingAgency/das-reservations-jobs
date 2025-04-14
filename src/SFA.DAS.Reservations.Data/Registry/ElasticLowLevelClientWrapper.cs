using System.Collections.Generic;
using System.Threading.Tasks;
using Elasticsearch.Net;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Data.Registry
{
    public class ElasticLowLevelClientWrapper(IElasticLowLevelClient client) : IElasticLowLevelClientWrapper
    {
        public async Task<StringResponse> Search(string name, string query)
        {
            var data = PostData.String(query);

            var searchAsync = await client.SearchAsync<StringResponse>(name, data);
            return searchAsync;
        }

        public async Task<ElasticSearchResponse> Create(string name, string id, string item)
        {
            var data = PostData.String(item);
            
            var response = await client.CreateAsync<ElasticSearchResponse>(name, id, data);

            return response;
        }

        public async Task DeleteDocument(string name, string id)
        {
            await client.DeleteAsync<ElasticSearchResponse>(name, id);
        }

        public async Task DeleteIndices(string name)
        {
            await client.Indices.DeleteAsync<ElasticSearchResponse>(name);
        }

        public async Task DeleteByQuery(string name, string query)
        {
            var data = PostData.String(query);

            await client.DeleteByQueryAsync<ElasticSearchResponse>(name, data);
        }

        public async Task UpdateByQuery(string name, string query)
        {
            var data = PostData.String(query);
            
            await client.UpdateByQueryAsync<ElasticSearchResponse>(name, data);
        }
        public async Task CreateIndicesWithMapping(string name, string mapping)
        {
            var data = PostData.String(mapping);

            await client.Indices.CreateAsync<ElasticSearchResponse>(name, data);
        }

        public async Task CreateMany(string name, IEnumerable<string> items)
        {
            var data = PostData.MultiJson(items);

            await client.BulkAsync<ElasticSearchResponse>(name, data);
        }
        
    }
}