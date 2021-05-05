using System;
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

        [Test]
        public void ThenWillThrowExceptionIfReservationIdIsInvalid()
        {
            //Arrange
            var reservationId = Guid.Empty;

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateReservationStatus(reservationId));
        }

        [Test]
        public async Task ThenReservationStatusIsNotChangedIfNoReservationFoundForAGivenId()
        {
            //Arrange
            var reservationId = Guid.NewGuid();
            Reservation reservation = null;

            _repository
                .Setup(x => x.GetReservationById(It.IsAny<Guid>()))
                .Returns(Task.FromResult(reservation));

            //Act
            await _service.UpdateReservationStatus(reservationId);

            _reservationIndex.Verify(r => r.SaveReservationStatus(reservationId, It.IsAny<ReservationStatus>()), Times.Never);
        }
        
        [Test, AutoData]
        public async Task ThenIfReservationHasStatusChangedWillSaveWithStatusDeleted(
            Guid reservationId,
            DateTime confirmedDate,
            long cohortId,
            long draftApprenticeshipId)
        {
            //Arrange
            _repository.Setup(x => x.GetReservationById(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new Reservation
                {
                    Status = (short)ReservationStatus.Change
                }));

            //Act
            await _service.UpdateReservationStatus(
                reservationId,
                ReservationStatus.Deleted,
                confirmedDate,
                cohortId,
                draftApprenticeshipId);

            //Assert
            _repository.Verify(r => r.Update(
                reservationId,
                ReservationStatus.Deleted,
                confirmedDate,
                cohortId,
                draftApprenticeshipId),
                Times.Once);

            _reservationIndex.Verify(r => r.SaveReservationStatus(reservationId, ReservationStatus.Deleted), Times.Once);
        }

        [Test, AutoData]
        public async Task ThenIfReservationIsNotStatusChangedWillSaveWithStatusPending(
            Guid reservationId,
            DateTime confirmedDate,
            long cohortId,
            long draftApprenticeshipId)
        {
            //Arrange
            _repository.Setup(x => x.GetReservationById(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new SFA.DAS.Reservations.Domain.Entities.Reservation
                {
                    Status = (short)ReservationStatus.Confirmed
                }));

            //Act
            await _service.UpdateReservationStatus(
                reservationId,
                confirmedDate,
                cohortId,
                draftApprenticeshipId);

            //Assert
            _repository.Verify(r => r.Update(
                    reservationId,
                    ReservationStatus.Pending,
                    confirmedDate,
                    cohortId,
                    draftApprenticeshipId),
                Times.Once);

            _reservationIndex.Verify(r => r.SaveReservationStatus(reservationId, ReservationStatus.Pending), Times.Once);
        }

    }
}
