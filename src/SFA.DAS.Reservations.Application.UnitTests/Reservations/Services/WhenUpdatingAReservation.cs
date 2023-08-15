using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenUpdatingAReservation
    {
        private ReservationService _service;
        private Mock<IReservationRepository> _repository;
        private Mock<IReservationIndexRepository> _reservationIndex;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IReservationRepository>();
            _reservationIndex = new Mock<IReservationIndexRepository>();

            _service = new ReservationService(
                _repository.Object,
                _reservationIndex.Object, 
                Mock.Of<IProviderPermissionRepository>(), 
                Mock.Of<ILogger<ReservationService>>());
        }

        [Test, AutoData]
        public async Task ThenWillSaveStatus(
            Guid reservationId,
            ReservationStatus status,
            DateTime confirmedDate,
            long cohortId,
            long draftApprenticeshipId)
        {
            //Act
            await _service.UpdateReservationStatus(
                reservationId, 
                status, 
                confirmedDate, 
                cohortId, 
                draftApprenticeshipId);

            //Assert
            _repository.Verify(r => r.Update(
                reservationId, 
                status, 
                confirmedDate, 
                cohortId, 
                draftApprenticeshipId), 
                Times.Once);
            _reservationIndex.Verify(r => r.SaveReservationStatus(reservationId, status), Times.Once);
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
            _repository.Verify(r => r.Update(
                It.IsAny<Guid>(), 
                It.IsAny<ReservationStatus>(),
                It.IsAny<DateTime>(),
                It.IsAny<long>(),
                It.IsAny<long>()), 
                Times.Never);
            _reservationIndex.Verify(r => r.SaveReservationStatus(It.IsAny<Guid>(), It.IsAny<ReservationStatus>()), Times.Never);
        }

        [Test]
        public async Task Then_Will_Not_Update_The_Index_If_The_Database_Update_Fails()
        {
            //Arrange
            var reservationId = Guid.NewGuid();
            var status = ReservationStatus.Confirmed;
            _repository.Setup(r => r.Update(
                    It.IsAny<Guid>(), 
                    It.IsAny<ReservationStatus>(),
                    null,
                    null,
                    null))
                .ThrowsAsync(new InvalidOperationException());

            //Act
            await _service.RefreshReservationIndex();
            
            //Assert
            _reservationIndex.Verify(r => r.SaveReservationStatus(It.IsAny<Guid>(), It.IsAny<ReservationStatus>()), Times.Never);
        }
    }
}
