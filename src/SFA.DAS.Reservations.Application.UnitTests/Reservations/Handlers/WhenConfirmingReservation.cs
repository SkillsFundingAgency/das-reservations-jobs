using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Handlers
{
    public class WhenConfirmingReservation
    {
        private ConfirmReservationHandler _handler;
        private Mock<IReservationService> _service;

        [SetUp]
        public void Arrange()
        {
            _service = new Mock<IReservationService>();
            _handler = new ConfirmReservationHandler(_service.Object);
        }

        [Test]
        public async Task ThenSetsReservationStatusToConfirmed()
        {
            //Arrange
            var reservationId = Guid.NewGuid();

            //Act
            await _handler.Handle(reservationId);

            //Assert
            _service.Verify(s => s.UpdateReservationStatus(reservationId, ReservationStatus.Confirmed), Times.Once);
        }

        
        [Test]
        public void ThenWillThrowExceptionIfReservationIdIsInvalid()
        {
            //Arrange
            var reservationId = Guid.Empty;

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(reservationId));

            //Assert
            Assert.AreEqual("reservationId", exception.ParamName);
            _service.Verify(s => s.UpdateReservationStatus(It.IsAny<Guid>(), It.IsAny<ReservationStatus>()), Times.Never);
        }
    }
}
