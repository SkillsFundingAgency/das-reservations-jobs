using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.ReservationIndexRepository
{
    public class WhenCreatingAnIndex
    {
        private IElasticClient _client;
        private Mock<IIndexRegistry> _registryMock;
        private Data.Repository.ReservationIndexRepository _repository;

        [SetUp]
        public void Init()
        {
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
            var connectionSettings = new ConnectionSettings(pool, new InMemoryConnection())
                .PrettyJson()
                .DisableDirectStreaming();

           _client = new ElasticClient(connectionSettings);

            _registryMock = new Mock<IIndexRegistry>();
            _repository = new Data.Repository.ReservationIndexRepository(_client, _registryMock.Object, new ReservationJobsEnvironment("LOCAL"));
        }

        [Test]
        public async Task ThenWillIndexManyReservationAtOnce()
        {
            //Act
            await _repository.CreateIndex();

            //Assert
            _registryMock.Verify(r => r.Add(It.Is<string>(s => s.StartsWith(_repository.IndexNamePrefix))), Times.Once);
        }
    }
}
