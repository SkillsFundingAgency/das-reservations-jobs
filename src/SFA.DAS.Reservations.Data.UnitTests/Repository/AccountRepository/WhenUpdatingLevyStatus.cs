using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.AccountRepository
{
    public class WhenUpdatingLevyStatus
    {
        private Data.Repository.AccountRepository _accountRepository;
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
            
        }

        [Test]
        public async Task Then_The_Levy_Status_Of_The_Account_Is_Updated()
        {
            //Arrange
            var expectedAccount = new Account
            {
                Id = 123,
                IsLevy = false
            };

            _dataContext.Setup(x => x.Accounts.FindAsync(expectedAccount.Id))
                .ReturnsAsync(expectedAccount);
            _dataContext.Setup(x => x.Database)
                .Returns(_dataFacade.Object);
            _accountRepository = new Data.Repository.AccountRepository(_dataContext.Object, Mock.Of<ILogger<Data.Repository.AccountRepository>>());

            //Act
            await _accountRepository.UpdateLevyStatus(new Account {Id = 123, IsLevy = true});
            
            //Assert
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
            expectedAccount.IsLevy.Should().BeTrue();
        }
        
        [Test]
        public void Then_If_The_Entity_Does_Not_Exist_An_Exception_Is_Thrown()
        {
            _dataContext.Setup(x => x.Accounts.FindAsync(It.IsAny<long>()))
                .ReturnsAsync((Account) null);
            _dataContext.Setup(x => x.Database)
                .Returns(_dataFacade.Object);
            _accountRepository = new Data.Repository.AccountRepository(_dataContext.Object, Mock.Of<ILogger<Data.Repository.AccountRepository>>());
            
            Assert.ThrowsAsync<DbUpdateException>(() => _accountRepository.UpdateLevyStatus(new Account{Id = 54, IsLevy = false}));

            //Assert
            _dataContext.Verify(x => x.SaveChanges(), Times.Never);
        }
    }
}