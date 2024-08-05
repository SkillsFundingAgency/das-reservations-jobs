using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;


namespace SFA.DAS.Reservations.Data.UnitTests.Repository.ReservationRepository
{
    public class WhenUpdatingAReservation
    {
        private Data.Repository.ReservationRepository _reservationRepository;
        private Mock<IReservationsDataContext> _dataContext;
        private Mock<DatabaseFacade> _dataFacade;
        private Mock<DbContext> _dbContext;
        private Mock<IDbContextTransaction> _dbContextTransaction;
        private Reservation _reservationEntity;

        [SetUp]
        public void Arrange()
        {
            _reservationEntity = new Reservation
            {
               Id = Guid.NewGuid(),
               Status = 1,
               ConfirmedDate = DateTime.UtcNow,
               CohortId = 1,
               DraftApprenticeshipId = 1
            };

            _dbContextTransaction = new Mock<IDbContextTransaction>();
            _dbContext = new Mock<DbContext>();
            _dataContext = new Mock<IReservationsDataContext>();
            _dataFacade = new Mock<DatabaseFacade>(_dbContext.Object);
            _dataFacade.Setup(x => x.BeginTransaction()).Returns(_dbContextTransaction.Object);

            _dataContext.Setup(x => x.Reservations).ReturnsDbSet(new List<Reservation>
            {
                _reservationEntity
            });

            _dataContext.Setup(x => x.Reservations.FindAsync(_reservationEntity.Id))
                .ReturnsAsync(_reservationEntity);

            _dataContext.Setup(x => x.Database)
                .Returns(_dataFacade.Object);

            _reservationRepository = new Data.Repository.ReservationRepository(_dataContext.Object);
        }

        [Test]
        public async Task Then_If_Reservation_Exists_Its_Status_Is_Updated()
        {
            //Act
            await _reservationRepository.Update(_reservationEntity.Id, ReservationStatus.Completed);

            //Assert 
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
            _reservationEntity.Status.Should().Be((short) ReservationStatus.Completed);
        }

        [Test]
        public void Then_If_Reservation_Does_Not_Exists_An_Exception_Is_Thrown()
        {
            //Act + Assert
            var action = () => _reservationRepository.Update(Guid.NewGuid(), ReservationStatus.Confirmed);
            action.Should().ThrowAsync<InvalidOperationException>();
           
            _dataContext.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestCase(ReservationStatus.Completed)]
        [TestCase(ReservationStatus.Pending)]
        [TestCase(ReservationStatus.Deleted)]
        [TestCase(ReservationStatus.Change)]
        public void Then_If_The_Reservation_Status_Being_Changed_To_Pending_When_It_Is_Not_Confirmed_And_Has_No_Audit_Values_An_Exception_Is_Thrown(ReservationStatus status)
        {
            var reservationId = Guid.NewGuid();
            _reservationEntity = new Reservation
            {
                Id = reservationId,
                IsLevyAccount = false,
                Status = (short)status
            };
            _dataContext.Setup(x => x.Reservations.FindAsync(_reservationEntity.Id)).ReturnsAsync(_reservationEntity);

            var action = () => _reservationRepository.Update(reservationId, ReservationStatus.Pending);
            action.Should().ThrowAsync<DbUpdateException>();
            
            _dataContext.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestCase(ReservationStatus.Completed)]
        [TestCase(ReservationStatus.Pending)]
        [TestCase(ReservationStatus.Deleted)]
        [TestCase(ReservationStatus.Change)]
        public async Task Then_The_Reservation_Status_Check_Is_Ignored_For_Levy_Reservations(ReservationStatus status)
        {
            var reservationId = Guid.NewGuid();
            _reservationEntity = new Reservation
            {
                Id = reservationId,
                IsLevyAccount = true,
                Status = (short)status
            };
            _dataContext.Setup(x => x.Reservations.FindAsync(_reservationEntity.Id)).ReturnsAsync(_reservationEntity);

            await _reservationRepository.Update(reservationId, ReservationStatus.Pending);
            
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
        }
        
        [Test]
        public async Task Then_Reservation_Is_Set_To_Pending_If_It_Is_Confirmed_And_Audit_Fields_Set_To_Null()
        {
            //Act
            await _reservationRepository.Update(_reservationEntity.Id, ReservationStatus.Pending);

            //Assert 
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
            _reservationEntity.Status.Should().Be((short) ReservationStatus.Pending);
            _reservationEntity.ConfirmedDate.Should().BeNull();
            _reservationEntity.CohortId.Should().BeNull();
            _reservationEntity.DraftApprenticeshipId.Should().BeNull();
        }

        [Test, AutoData]
        public async Task And_Status_Confirmed_Then_Reservation_Stamped_With_CohortId_And_DraftApprenticeshipId(
            DateTime confirmedDate, 
            long cohortId,
            long draftApprenticeshipId)
        {
            //Act
            await _reservationRepository.Update(
                _reservationEntity.Id, 
                ReservationStatus.Confirmed, 
                confirmedDate, 
                cohortId, 
                draftApprenticeshipId);

            //Assert 
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
            _reservationEntity.Status.Should().Be((short) ReservationStatus.Confirmed);
            _reservationEntity.ConfirmedDate.Should().Be(confirmedDate);
            _reservationEntity.CohortId.Should().Be(cohortId);
            _reservationEntity.DraftApprenticeshipId.Should().Be(draftApprenticeshipId);
        }

        [Test, AutoData]
        public async Task And_Status_Confirmed_Then_Reservation_Status_Not_Updated_For_Changed_Records(
           DateTime confirmedDate,
           long cohortId,
           long draftApprenticeshipId)
        {
            // Arrange
            _reservationEntity.Status = 4;

            //Act
            await _reservationRepository.Update(
                _reservationEntity.Id,
                ReservationStatus.Confirmed,
                confirmedDate,
                cohortId,
                draftApprenticeshipId);

            //Assert 
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
            _reservationEntity.Status.Should().Be((short)ReservationStatus.Change);
            _reservationEntity.ConfirmedDate.Should().Be(confirmedDate);
            _reservationEntity.CohortId.Should().Be(cohortId);
            _reservationEntity.DraftApprenticeshipId.Should().Be(draftApprenticeshipId);
        }

        [Test, AutoData]
        public async Task And_Status_Deleted_Then_Reservation_Status_Updated_For_Changed_Records(
           DateTime confirmedDate,
           long cohortId,
           long draftApprenticeshipId)
        {
            // Arrange
            _reservationEntity.Status = 4;

            //Act
            await _reservationRepository.Update(
                _reservationEntity.Id,
                ReservationStatus.Deleted,
                confirmedDate,
                cohortId,
                draftApprenticeshipId);

            //Assert 
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
            _reservationEntity.Status.Should().Be((short)ReservationStatus.Deleted);
            _reservationEntity.ConfirmedDate.Should().Be(null);
            _reservationEntity.CohortId.Should().Be(null);
            _reservationEntity.DraftApprenticeshipId.Should().Be(null);
        }
    }
}
