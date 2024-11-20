using System.IO;
using System.Reflection;
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
            //var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, ".."));
            var rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            LastIndexSearchQuery = File.ReadAllText(Path.Combine(rootDirectory,"ElasticQueries/LatestIndexSearchQuery.json"));
            ReservationIndexMapping = File.ReadAllText(Path.Combine(rootDirectory,"ElasticQueries/CreateReservationIndex.json"));
            UpdateReservationStatus = File.ReadAllText(Path.Combine(rootDirectory,"ElasticQueries/UpdateReservationStatus.json"));
            DeleteReservationsByQuery = File.ReadAllText(Path.Combine(rootDirectory,"ElasticQueries/DeleteReservationsByQuery.json"));
        }
    }
}