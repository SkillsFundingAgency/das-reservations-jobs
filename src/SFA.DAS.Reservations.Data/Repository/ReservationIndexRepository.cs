using System;
using System.Collections.Generic;
using System.Linq;
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
                .Query(rq => rq.Term(new Field("reservationId"),id))
                .Script(c =>
                    c.Source("ctx._source.status = params.status")
                        .Params(p => p.Add("status", status)))
                .Refresh());
        }

        public async Task DeleteReservationsFromIndex(uint ukPrn, long accountLegalEntityId)
        {
            await _client.DeleteByQueryAsync<IndexedReservation>(q =>
                q.Index(_registry.CurrentIndexName)
                    .Query(rq => rq.MatchPhrasePrefix(f => f.Field("id")
                        .Query($"{ukPrn}_{accountLegalEntityId}_")))
                    .Refresh());
        }

        public async Task CreateIndex()
        {
            await _registry.Add(IndexNamePrefix + Guid.NewGuid());
        }
    }
}