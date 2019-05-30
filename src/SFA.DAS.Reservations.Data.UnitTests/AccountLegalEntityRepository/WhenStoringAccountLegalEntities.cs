using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.AccountLegalEntityRepository
{
    public class WhenStoringAccountLegalEntities
    {
        private Repository.AccountLegalEntityRepository _accountLegalEntityRepository;
        private Mock<IReservationsDataContext> _dataContext;
        private Mock<DatabaseFacade> _dataFacade;
        private Mock<DbContext> _dbContext;
        private Mock<IDbContextTransaction> _dbContextTransaction;

        [SetUp]
        public void Arrange()
        {
            

            _dbContextTransaction = new Mock<IDbContextTransaction>();
            _dbContext = new Mock<DbContext>();
            _dataContext = new Mock<IReservationsDataContext>();
            _dataFacade = new Mock<DatabaseFacade>(_dbContext.Object);
            _dataFacade.Setup(x => x.BeginTransaction()).Returns(_dbContextTransaction.Object);
            _dataContext.Setup(x => x.AccountLegalEntities.AddAsync(It.IsAny<AccountLegalEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityEntry<AccountLegalEntity>)null);
            _dataContext.Setup(x => x.Database)
                .Returns(_dataFacade.Object);

            _accountLegalEntityRepository = new Repository.AccountLegalEntityRepository(_dataContext.Object);
        }

        [Test]
        public async Task Then_The_AccountLegalEntity_Is_Added_To_The_Repository()
        {
            //Arrange
            var expectedAccountLegalEntity = new AccountLegalEntity
            {
                Id = Guid.NewGuid(),
                ReservationLimit = 1,
                AccountId = 1234,
                LegalEntityId = 543,
                AccountLegalEntityId = 5677,
                AgreementSigned = true,
                AccountLegalEntityName = "Test"
            };

            //Act
            await _accountLegalEntityRepository.Add(expectedAccountLegalEntity);

            //Assert 
            _dataContext.Verify(x => x.AccountLegalEntities.AddAsync(It.Is<AccountLegalEntity>(
                c => c.Id.Equals(expectedAccountLegalEntity.Id) &&
                     c.AccountId.Equals(expectedAccountLegalEntity.AccountId) &&
                     c.AgreementSigned.Equals(expectedAccountLegalEntity.AgreementSigned) &&
                     c.AccountLegalEntityId.Equals(expectedAccountLegalEntity.AccountLegalEntityId) &&
                     c.AccountLegalEntityName.Equals(expectedAccountLegalEntity.AccountLegalEntityName) &&
                     c.LegalEntityId.Equals(expectedAccountLegalEntity.LegalEntityId) &&
                     c.ReservationLimit.Equals(expectedAccountLegalEntity.ReservationLimit) 
                     ), It.IsAny<CancellationToken>()), Times.Once);
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);

        }
    }
}
