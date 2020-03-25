using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.ReservationIndexRepository
{
    public class WhenChangingReservationStatus
    {
        private const string ExpectedEnvironmentName = "local";
        private const string ExpectedCurrentIndexName = "local-reservation-latest";
        private const string ExpectedReservationStatusQuery = "{status} {reservationId}";
        
        private Mock<IElasticLowLevelClientWrapper> _elasticClientWrapper;
        private Data.Repository.ReservationIndexRepository _reservationIndexRepository;
        private Mock<IIndexRegistry> _indexRegistry;
        private Mock<IElasticSearchQueries> _elasticSearchQueries;

        [SetUp] 
        public void Arrange()
        {
            _elasticClientWrapper = new Mock<IElasticLowLevelClientWrapper>();
            _indexRegistry = new Mock<IIndexRegistry>();
            _elasticSearchQueries = new Mock<IElasticSearchQueries>();
            _elasticSearchQueries.Setup(x => x.UpdateReservationStatus).Returns(ExpectedReservationStatusQuery);

            _indexRegistry.Setup(x => x.CurrentIndexName).Returns(ExpectedCurrentIndexName);
            
            _reservationIndexRepository = new Data.Repository.ReservationIndexRepository(_elasticClientWrapper.Object,_indexRegistry.Object, _elasticSearchQueries.Object, new ReservationJobsEnvironment(ExpectedEnvironmentName));
            
        }
        
        [Test]
        public async Task Then_The_Reservation_Statuses_Are_Updated()
        {
            //Arrange
            var reservationId = Guid.NewGuid();
            var reservationStatus = ReservationStatus.Confirmed;
            
            //Act
            await _reservationIndexRepository.SaveReservationStatus(reservationId, reservationStatus);
            
            //Assert
            _elasticClientWrapper.Verify(x=>x.UpdateByQuery(ExpectedCurrentIndexName, $"{(short)reservationStatus} {reservationId}"));
        }
    }
}