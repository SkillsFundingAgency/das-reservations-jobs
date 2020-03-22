using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch
{
    public interface IIndexRegistry
    {
        string CurrentIndexName { get; }
        Task Add(string indexName);
        Task DeleteOldIndices(uint daysOld);
    }
}
