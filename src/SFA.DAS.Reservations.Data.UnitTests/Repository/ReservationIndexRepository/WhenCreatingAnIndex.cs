using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.ReservationIndexRepository
{
    public class WhenCreatingAnIndex
    {
        private const string ExpectedMapping = "some mapping";
        private Mock<IElasticLowLevelClientWrapper> _client;
        private Mock<IIndexRegistry> _registryMock;
        private Data.Repository.ReservationIndexRepository _repository;
        private Mock<IElasticSearchQueries> _elasticSearchQueries;

        [SetUp]
        public void Init()
        {
            _client = new Mock<IElasticLowLevelClientWrapper>();
            _registryMock = new Mock<IIndexRegistry>();
            _elasticSearchQueries = new Mock<IElasticSearchQueries>();
            _elasticSearchQueries.Setup(x => x.ReservationIndexMapping).Returns(ExpectedMapping);
            
            _repository = new Data.Repository.ReservationIndexRepository(_client.Object, _registryMock.Object, _elasticSearchQueries.Object, new ReservationJobsEnvironment("LOCAL"));
        }

        [Test]
        public async Task ThenWillIndexManyReservationAtOnce()
        {
            //Act
            await _repository.CreateIndex();

            //Assert
            _client.Verify(x=>x.CreateIndicesWithMapping(It.Is<string>(c=>c.StartsWith(_repository.IndexNamePrefix)),ExpectedMapping),Times.Once);
            _registryMock.Verify(r => r.Add(It.Is<string>(s => s.StartsWith(_repository.IndexNamePrefix))), Times.Once);
        }
    }
}
