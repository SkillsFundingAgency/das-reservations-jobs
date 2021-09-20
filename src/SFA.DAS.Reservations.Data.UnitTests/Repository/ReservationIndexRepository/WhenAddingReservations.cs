using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.ReservationIndexRepository
{
    public class WhenAddingReservations
    {
        private const string ExpectedIndexName = "Test";
        private Mock<IElasticLowLevelClientWrapper> _clientMock;
        private Mock<IIndexRegistry> _registryMock;
        private Data.Repository.ReservationIndexRepository _repository;
        private Mock<IElasticSearchQueries> _elasticSearchQueries;

        [SetUp]
        public void Init()
        {
            _clientMock = new Mock<IElasticLowLevelClientWrapper>();
            _registryMock = new Mock<IIndexRegistry>();
            _elasticSearchQueries = new Mock<IElasticSearchQueries>();
            
            _repository = new Data.Repository.ReservationIndexRepository(_clientMock.Object, _registryMock.Object, _elasticSearchQueries.Object, new ReservationJobsEnvironment("LOCAL"));

            _registryMock.SetupGet(x => x.CurrentIndexName).Returns(ExpectedIndexName);
        }

        [Test]
        public async Task ThenWillIndexManyReservationAtOnce()
        {
            //Arrange
            var firstReservation = Guid.NewGuid();
            var indexedProviderId = 12345u;
            var accountLegalEntityId = 54321;
            var reservations = new List<IndexedReservation>
            {
                new IndexedReservation {ReservationId = firstReservation, Status = 1, IndexedProviderId = indexedProviderId, AccountLegalEntityId = accountLegalEntityId},
                new IndexedReservation {ReservationId = Guid.NewGuid(), Status = 1}
            };

            //Act
            await _repository.Add(reservations);

            //Assert
            _clientMock.Verify(c => 
                c.CreateMany(
                    ExpectedIndexName,
                    It.Is<IEnumerable<string>>(x=>JsonConvert.DeserializeObject<IndexedReservation>(
                        x.Skip(1).First()).ReservationId.Equals(firstReservation) 
                                           && x.First().Equals(@"{ ""index"":{""_id"":""" + indexedProviderId + "_" + accountLegalEntityId + "_"+ firstReservation + @"""} }")))
                , Times.Once);
        }


    }
}
