using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.ReservationIndexRepository
{
    public class WhenDeletingOldIndices
    {
        private Mock<IElasticLowLevelClientWrapper> _clientMock;
        private Mock<IIndexRegistry> _registryMock;
        private Data.Repository.ElasticReservationIndexRepository _repository;
        private Mock<IElasticSearchQueries> _elasticSearchQueries;

        [SetUp]
        public void Init()
        {
            _clientMock = new Mock<IElasticLowLevelClientWrapper>();
            _registryMock = new Mock<IIndexRegistry>();
            _elasticSearchQueries = new Mock<IElasticSearchQueries>();
            
            _repository = new Data.Repository.ElasticReservationIndexRepository(_clientMock.Object, _registryMock.Object, _elasticSearchQueries.Object, new ReservationJobsEnvironment("LOCAL"));
        }

        [Test]
        public async Task ThenWillDeleteIndicesOlderThanGivenDaysOld()
        {
            //Arrange
            const uint daysOld = 2;
            
            //Act
            await _repository.DeleteIndices(daysOld);

            //Assert
            _registryMock.Verify(r => r.DeleteOldIndices(daysOld), Times.Once);
        }
    }
}
