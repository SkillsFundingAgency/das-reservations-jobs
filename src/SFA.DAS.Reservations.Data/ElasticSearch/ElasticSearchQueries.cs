using System.IO;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Data.ElasticSearch
{
    public class ElasticSearchQueries : IElasticSearchQueries
    {
        public string ReservationIndexLookupName => "-reservations-index-registry";
        public string LastIndexSearchQuery { get; }
        
        public ElasticSearchQueries()
        {
            LastIndexSearchQuery = File.ReadAllText("ElasticSearch/LatestIndexSearchQuery.json");
            
        }
    }
}