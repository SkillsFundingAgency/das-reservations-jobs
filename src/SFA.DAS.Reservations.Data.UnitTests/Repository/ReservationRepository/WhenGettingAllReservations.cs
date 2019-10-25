using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.ReservationRepository
{
    public class WhenGettingAllReservations
    {
        private Data.Repository.ReservationRepository _reservationRepository;
        private Mock<IReservationsDataContext> _dataContext;
        private Mock<DatabaseFacade> _dataFacade;
        private Mock<DbContext> _dbContext;
        private Mock<IDbContextTransaction> _dbContextTransaction;
        private IEnumerable<Reservation> _expectedReservations;

        [SetUp]
        public void Arrange()
        {
            _expectedReservations = new List<Reservation>
            {
                new Reservation { Id = Guid.NewGuid(), Status = 1 },
                new Reservation { Id = Guid.NewGuid(), Status = 1 },
                new Reservation { Id = Guid.NewGuid(), Status = 1 }
            };

            _dbContextTransaction = new Mock<IDbContextTransaction>();
            _dbContext = new Mock<DbContext>();
            _dataContext = new Mock<IReservationsDataContext>();
            _dataFacade = new Mock<DatabaseFacade>(_dbContext.Object);
            _dataFacade.Setup(x => x.BeginTransaction()).Returns(_dbContextTransaction.Object);

            _dataContext.Setup(x => x.Reservations).ReturnsDbSet(_expectedReservations);
            _dataContext.Setup(x => x.Database).Returns(_dataFacade.Object);

            _reservationRepository = new Data.Repository.ReservationRepository(_dataContext.Object);
        }

        [Test]
        public void ThenReturnsAllReservations()
        {
            //Act
            var reservations = _reservationRepository.GetAll();

            //Assert 
            reservations.Should().BeEquivalentTo(_expectedReservations);
        }
    }
}
