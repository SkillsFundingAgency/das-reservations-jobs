using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ReservationIndexRepository : IReservationIndexRepository
    {
        public string IndexNamePrefix { get; }

        private readonly IElasticClient _client;
        private readonly IIndexRegistry _registry;

        public ReservationIndexRepository(IElasticClient client, IIndexRegistry registry,
            ReservationJobsEnvironment environment)
        {
            IndexNamePrefix = $"{environment.EnvironmentName}-reservations-";
            _client = client;
            _registry = registry;
        }

        public async Task Add(IEnumerable<IndexedReservation> reservations)
        {
            await _client.IndexManyAsync(reservations, _registry.CurrentIndexName);
        }

        public async Task DeleteMany(IList<IndexedReservation> reservationIndex)
        {
            await _client.DeleteManyAsync(reservationIndex, _registry.CurrentIndexName);
        }

        public async Task Add(IndexedReservation reservation)
        {
            await _client.IndexAsync(new IndexRequest<IndexedReservation>(reservation, _registry.CurrentIndexName));
        }

        public async Task DeleteIndices(uint daysOld)
        {
            await _registry.DeleteOldIndices(daysOld);
        }

        public async Task SaveReservationStatus(Guid id, ReservationStatus status)
        {
            await _client
                .UpdateByQueryAsync<IndexedReservation>(q => q.Index(_registry.CurrentIndexName)
                .Query(rq => rq.MatchPhrase(f=>f.Field("reservationId")
                    .Query(id.ToString().ToLower())))
                .Script($"ctx._source.status = {(short)status}")
                .WaitForCompletion()
                .Refresh());
        }

        public async Task DeleteReservationsFromIndex(uint ukPrn, long accountLegalEntityId)
        {
            await _client.DeleteByQueryAsync<IndexedReservation>(q =>
                q.Index(_registry.CurrentIndexName)
                    .Query(rq => rq.MatchPhrasePrefix(f => f.Field("id")
                        .Query($"{ukPrn}_{accountLegalEntityId}_")))
                    .WaitForCompletion()
                    .Refresh());
        }

        public async Task CreateIndex()
        {
            var indexName = IndexNamePrefix + Guid.NewGuid();

            await _client.Indices.CreateAsync(indexName, c =>
                c.Map<IndexedReservation>(r =>
                    r.Properties(p =>p
                        .Keyword(k => k.Name(n => n.CourseDescription))
                        .Keyword(k => k.Name(n => n.CourseId))
                        )));

            await _registry.Add(indexName);
        }
    }
}