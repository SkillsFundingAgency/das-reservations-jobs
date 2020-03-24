using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ReservationIndexRepository : IReservationIndexRepository
    {
        public string IndexNamePrefix { get; }

        private readonly IElasticLowLevelClientWrapper _client;
        private readonly IIndexRegistry _registry;
        private readonly IElasticSearchQueries _elasticSearchQueries;

        public ReservationIndexRepository(IElasticLowLevelClientWrapper client, IIndexRegistry registry, IElasticSearchQueries elasticSearchQueries,
            ReservationJobsEnvironment environment)
        {
            IndexNamePrefix = $"{environment.EnvironmentName}-reservations-";
            _client = client;
            _registry = registry;
            _elasticSearchQueries = elasticSearchQueries;
        }

        public async Task Add(IEnumerable<IndexedReservation> reservations)
        {
            var listOfJsonReservations = new List<string>();
            foreach (var reservation in reservations)
            {
                listOfJsonReservations.Add(@"{ ""index"":{""_id"":"""+ reservation.Id + @"""} }");
                listOfJsonReservations.Add(JsonConvert.SerializeObject(reservation));
            }
            listOfJsonReservations.Add(Environment.NewLine);
            
            await _client.CreateMany(_registry.CurrentIndexName, listOfJsonReservations);
        }

        public async Task DeleteIndices(uint daysOld)
        {
            await _registry.DeleteOldIndices(daysOld);
        }

        public async Task SaveReservationStatus(Guid id, ReservationStatus status)
        {
            var query = _elasticSearchQueries.UpdateReservationStatus.Replace("{status}",status.ToString())
                .Replace("{reservationId}",id.ToString());
            
            await _client.UpdateByQuery(_registry.CurrentIndexName, query);
        }

        public async Task DeleteReservationsFromIndex(uint ukPrn, long accountLegalEntityId)
        {
            var query = _elasticSearchQueries.DeleteReservationsByQuery
                .Replace("{ukPrn}", ukPrn.ToString())
                .Replace("{accountLegalEntityId}", accountLegalEntityId.ToString());

            await _client.DeleteByQuery(_registry.CurrentIndexName, query);
        }

        public async Task CreateIndex()
        {
            var indexName = IndexNamePrefix + Guid.NewGuid();
            var mapping = _elasticSearchQueries.ReservationIndexMapping;
            
            await _client.CreateIndicesWithMapping(indexName, mapping);
            
            await _registry.Add(indexName);
        }
    }
}