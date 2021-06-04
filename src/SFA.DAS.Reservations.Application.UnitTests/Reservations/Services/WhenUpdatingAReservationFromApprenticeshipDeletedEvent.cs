using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenUpdatingAReservationFromApprenticeshipDeletedEvent
    {
        private ReservationService _service;
        private Mock<IReservationRepository> _repository;
        private Mock<IReservationIndexRepository> _reservationIndex;
        private Mock<ILogger<ReservationService>> _logger;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IReservationRepository>();
            _reservationIndex = new Mock<IReservationIndexRepository>();
            _logger = new Mock<ILogger<ReservationService>>();

            _service = new ReservationService(
                _repository.Object,
                _reservationIndex.Object,
                Mock.Of<IProviderPermissionRepository>(),
                _logger.Object);
        }

        [Test]
        public void ThenWillThrowExceptionIfReservationIdIsInvalid()
        {
            //Arrange
            var reservationId = Guid.Empty;

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _service.GetReservation(reservationId));
        }

        [Test]
        public async Task ThenIfNoReservationFoundForAGivenIdReturnNull()
        {
            //Arrange
            var reservationId = Guid.NewGuid();
            Reservation reservation = null;

            _repository
                .Setup(x => x.GetReservationById(It.IsAny<Guid>()))
                .ReturnsAsync(reservation);

            //Act
            var result = await _service.GetReservation(reservationId);

            //Assert
            Assert.IsNull(result);


            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }
        
      

    }
}
