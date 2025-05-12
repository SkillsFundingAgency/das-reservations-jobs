using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ElasticReservationIndexRepository(
        IElasticLowLevelClientWrapper client,
        IIndexRegistry registry,
        IElasticSearchQueries elasticSearchQueries,
        ReservationJobsEnvironment environment)
        : IElasticReservationIndexRepository
    {
        public string IndexNamePrefix { get; } = $"{environment.EnvironmentName}-reservations-";

        public async Task Add(IEnumerable<IndexedReservation> reservations)
        {
            var listOfJsonReservations = new List<string>();

            foreach (var reservation in reservations)
            {
                listOfJsonReservations.Add(@"{ ""index"":{""_id"":""" + reservation.Id + @"""} }");
                listOfJsonReservations.Add(JsonConvert.SerializeObject(reservation));
            }
            
            var reservationBatches = BatchReservationDocs(listOfJsonReservations);

            foreach (var batch in reservationBatches)
            {
                await client.CreateMany(registry.CurrentIndexName, batch);
            }
        }

        public async Task DeleteIndices(uint daysOld)
        {
            await registry.DeleteOldIndices(daysOld);
        }

        public async Task SaveReservationStatus(Guid id, ReservationStatus status)
        {
            var query = elasticSearchQueries.UpdateReservationStatus.Replace("{status}",((short)status).ToString())
                .Replace("{reservationId}",id.ToString());
            
            await client.UpdateByQuery(registry.CurrentIndexName, query);
        }

        public async Task DeleteReservationsFromIndex(uint ukPrn, long accountLegalEntityId)
        {
            var query = elasticSearchQueries.DeleteReservationsByQuery
                .Replace("{ukPrn}", ukPrn.ToString())
                .Replace("{accountLegalEntityId}", accountLegalEntityId.ToString());

            await client.DeleteByQuery(registry.CurrentIndexName, query);
        }

        public async Task CreateIndex()
        {
            var indexName = IndexNamePrefix + Guid.NewGuid();
            var mapping = elasticSearchQueries.ReservationIndexMapping;
            
            await client.CreateIndicesWithMapping(indexName, mapping);
            
            await registry.Add(indexName);
        }

        private static IEnumerable<IEnumerable<string>> BatchReservationDocs(List<string> listOfJsonReservations)
        {
            var maxItems = 10000;
            return listOfJsonReservations.Select((item, inx) => new { item, inx })
                    .GroupBy(x => x.inx / maxItems)
                    .Select(g => g.Select(x => x.item));
        }

    }
}