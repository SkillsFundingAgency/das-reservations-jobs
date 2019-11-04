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

        public ReservationIndexRepository(IElasticClient client, IIndexRegistry registry, ReservationJobsEnvironment environment)
        {
            IndexNamePrefix = $"{environment.EnvironmentName}-reservations-";
            _client = client;
            _registry = registry;
        }

        public async Task Add(IEnumerable<IndexedReservation> reservations)
        {
            await _client.IndexManyAsync(reservations, _registry.CurrentIndexName);
        }

        public async Task Add(IndexedReservation reservation) 
        {
            await _client.IndexAsync(new IndexRequest<IndexedReservation>(reservation, _registry.CurrentIndexName));
        }

        public async Task DeleteIndices(uint daysOld)
        {
            await _registry.DeleteOldIndices(daysOld);
        }

        public async Task CreateIndex()
        {
            await _registry.Add(IndexNamePrefix + Guid.NewGuid());
        }
    }
}
