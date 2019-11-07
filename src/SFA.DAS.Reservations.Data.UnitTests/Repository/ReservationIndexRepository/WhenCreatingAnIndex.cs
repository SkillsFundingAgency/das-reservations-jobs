using System.Threading.Tasks;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.ReservationIndexRepository
{
    public class WhenCreatingAnIndex
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
        public async Task ThenWillIndexManyReservationAtOnce()
        {
            //Act
            await _repository.CreateIndex();

            //Assert
            _registryMock.Verify(r => r.Add(It.Is<string>(s => s.StartsWith(_repository.IndexNamePrefix))), Times.Once);
        }
    }
}
