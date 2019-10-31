using System.Threading.Tasks;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.ReservationIndexRepository
{
    public class WhenDeletingOldIndices
    {
        private Mock<IElasticClient> _clientMock;
        private Mock<IIndexRegistry> _registryMock;
        private Data.Repository.ReservationIndexRepository _repository;

        [SetUp]
        public void Init()
        {
            _clientMock = new Mock<IElasticClient>();
            _registryMock = new Mock<IIndexRegistry>();
            _repository = new Data.Repository.ReservationIndexRepository(_clientMock.Object, _registryMock.Object, new ReservationJobsEnvironment("LOCAL"));
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
