using System.Threading.Tasks;
using Elasticsearch.Net;

namespace SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch
{
    public interface IElasticLowLevelClientWrapper
    {
        
        Task<StringResponse> Search(string name, string query);
        Task<ElasticSearchResponse> Create(string name, string id, string item);
        Task DeleteDocument(string name, string id);
        Task DeleteIndices(string name);
    }
}