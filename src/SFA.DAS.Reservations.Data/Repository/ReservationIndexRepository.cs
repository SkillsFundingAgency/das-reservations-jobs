using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ReservationIndexRepository : IReservationIndexRepository
    {
        private const string IndexNamePrefix = "reservations-";

        private readonly IElasticClient _client;
        private readonly IIndexRegistry _registry;

        public ReservationIndexRepository(IElasticClient client, IIndexRegistry registry)
        {
            _client = client;
            _registry = registry;
        }

        public async Task Add(IEnumerable<ReservationIndex> reservations)
        {
            await _client.IndexManyAsync(reservations, _registry.CurrentIndexName);
        }

        public async Task Add(ReservationIndex reservation) 
        {
            await _client.IndexAsync(new IndexRequest<ReservationIndex>(reservation, _registry.CurrentIndexName));
        }

        public async Task CreateIndex()
        {
            await _client.Indices.CreateAsync(new CreateIndexRequest(IndexNamePrefix + Guid.NewGuid()));
        }

        //public async Task DeleteIndex(string indexName)
        //{
        //    await _client.Indices.DeleteAsync(Indices.Index(_indexName));
        //}
    }
}
