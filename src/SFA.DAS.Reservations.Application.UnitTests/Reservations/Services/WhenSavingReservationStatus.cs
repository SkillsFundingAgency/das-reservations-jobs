using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenSavingReservationStatus
    {
        private ReservationService _service;
        private Mock<IReservationRepository> _repository;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IReservationRepository>();

            _service = new ReservationService(
                _repository.Object, 
                Mock.Of<IReservationIndexRepository>(), 
                Mock.Of<IProviderPermissionRepository>(), 
                Mock.Of<ILogger<ReservationService>>());
        }

        [Test]
        public async Task ThenWillSaveStatus()
        {
            //Arrange
            var reservationId = Guid.NewGuid();
            var status = ReservationStatus.Confirmed;

            //Act
            await _service.UpdateReservationStatus(reservationId, status);

            //Assert
            _repository.Verify(r => r.SaveStatus(reservationId, status), Times.Once);
        }

        [Test]
        public void ThenWillThrowExceptionIfReservationIdIsInvalid()
        {
            //Arrange
            var reservationId = Guid.Empty;
            var status = ReservationStatus.Confirmed;

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateReservationStatus(reservationId, status));

            //Assert
            Assert.AreEqual("reservationId", exception.ParamName);
            _repository.Verify(r => r.SaveStatus(It.IsAny<Guid>(), It.IsAny<ReservationStatus>()), Times.Never);
        }
    }
}
