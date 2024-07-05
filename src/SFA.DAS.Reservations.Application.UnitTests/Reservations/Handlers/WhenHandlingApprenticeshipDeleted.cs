using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Handlers
{
    public class WhenHandlingApprenticeshipDeleted
    {
        private Mock<IReservationService> _reservationService;
        private ApprenticeshipDeletedHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _reservationService = new Mock<IReservationService>();
            _handler = new ApprenticeshipDeletedHandler(_reservationService.Object);
        }
        
        [Test]
        public async Task ThenIfReservationHasStatusChangedWillSaveWithStatusDeleted()
        {
            //Arrange
            Guid deletedEventGuid = Guid.NewGuid();

           _reservationService.Setup(x => x.GetReservation(deletedEventGuid))
                .ReturnsAsync(new Reservation
                {
                    Id = deletedEventGuid,
                    Status = ReservationStatus.Change
                });
            
            //Act
            await _handler.Handle(deletedEventGuid);

            //Assert
            _reservationService
                .Verify(service => service.UpdateReservationStatus(
                        deletedEventGuid,
                        ReservationStatus.Deleted,
                        null,
                        null,
                        null),
                    Times.Once);
        }

        [Test]
        public async Task ThenIfReservationIsNotStatusChangedWillSaveWithStatusPending()
        {
            //Arrange
            Guid deletedEventGuid = Guid.NewGuid();

            _reservationService.Setup(x => x.GetReservation(deletedEventGuid))
                .ReturnsAsync(new Reservation
                {
                    Id = deletedEventGuid,
                    Status = ReservationStatus.Confirmed
                });

            //Act
            await _handler.Handle(deletedEventGuid);

            //Assert
            _reservationService
                .Verify(service => service.UpdateReservationStatus(
                        deletedEventGuid,
                        ReservationStatus.Pending,
                        null,
                        null,
                        null),
                    Times.Once);
        }
    }
}