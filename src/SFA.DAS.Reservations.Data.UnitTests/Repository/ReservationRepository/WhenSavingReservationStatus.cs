using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class WhenSavingReservationStatus
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
                Status = 1
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
            await _reservationRepository.SaveStatus(_reservationEntity.Id, ReservationStatus.Completed);

            //Assert 
            Assert.AreEqual((short)ReservationStatus.Completed, _reservationEntity.Status);
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Test]
        public void Then_If_Reservation_Does_Not_Exists_An_Exception_Is_Thrown()
        {
            //Act + Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _reservationRepository.SaveStatus(Guid.NewGuid(), ReservationStatus.Confirmed));
           
            _dataContext.Verify(x => x.SaveChanges(), Times.Never);
        }
    }
}
