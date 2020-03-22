using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.ReservationIndexRepository
{
    public class WhenAddingReservations
    {
        private const string ExpectedIndexName = "Test";
        private Mock<IElasticClient> _clientMock;
        private Mock<IIndexRegistry> _registryMock;
        private Data.Repository.ReservationIndexRepository _repository;

        [SetUp]
        public void Init()
        {
            _clientMock = new Mock<IElasticClient>();
            _registryMock = new Mock<IIndexRegistry>();
            _repository = new Data.Repository.ReservationIndexRepository(_clientMock.Object, _registryMock.Object, new ReservationJobsEnvironment("LOCAL"));

            _registryMock.SetupGet(x => x.CurrentIndexName).Returns(ExpectedIndexName);
        }

        [Test]
        public async Task ThenWillIndexManyReservationAtOnce()
        {
            //Arrange
            var reservations = new List<IndexedReservation>
            {
                new IndexedReservation {ReservationId = Guid.NewGuid(), Status = 1},
                new IndexedReservation {ReservationId = Guid.NewGuid(), Status = 1}
            };

            //Act
            await _repository.Add(reservations);

            //Assert
            _clientMock.Verify(c => c.BulkAsync(It.Is<IBulkRequest>(r => 
                r.Operations.OfType<BulkIndexOperation<IndexedReservation>>()
                    .Select(o => o.Document)
                    .SequenceEqual(reservations)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenWillIndexManyReservationToCorrectIndex()
        {
            //Arrange
            var reservations = new List<IndexedReservation>
            {
                new IndexedReservation {ReservationId = Guid.NewGuid(), Status = 1},
                new IndexedReservation {ReservationId = Guid.NewGuid(), Status = 1}
            };

            //Act
            await _repository.Add(reservations);

            //Assert
            _clientMock.Verify(c => c.BulkAsync(It.Is<IBulkRequest>(r => r.Index.Name.Equals(ExpectedIndexName)), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
