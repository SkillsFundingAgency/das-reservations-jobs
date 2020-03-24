using System.IO;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Data.ElasticSearch
{
    public class ElasticSearchQueries : IElasticSearchQueries
    {
        public string ReservationIndexLookupName => "-reservations-index-registry";
        public string LastIndexSearchQuery { get; }
        public string ReservationIndexMapping { get; }
        public string UpdateReservationStatus { get; }
        public string DeleteReservationsByQuery { get; }

        public ElasticSearchQueries()
        {
            LastIndexSearchQuery = File.ReadAllText("ElasticQueries/LatestIndexSearchQuery.json");
            ReservationIndexMapping = File.ReadAllText("ElasticQueries/CreateReservationIndex.json");
            UpdateReservationStatus = File.ReadAllText("ElasticQueries/UpdateReservationStatus.json");
            DeleteReservationsByQuery = File.ReadAllText("ElasticQueries/DeleteReservationsByQuery.json");
        }
    }
}