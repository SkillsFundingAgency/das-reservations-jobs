using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Data.Registry
{
    public interface IIndexRegistry
    {
        string CurrentIndexName { get; }
        Task Add(string indexName);
    }
}
