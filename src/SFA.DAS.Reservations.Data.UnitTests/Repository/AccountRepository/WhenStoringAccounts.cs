using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.AccountRepository
{
    public class WhenStoringAccounts
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
        public async Task Then_The_Account_Is_Added_To_The_Repository_And_Committed()
        {
            //Arrange
            var expectedAccount = new Account
            {
                Id = 123,
                Name = "Test"
            };
            
            _dataContext.Setup(x => x.Accounts)
                .ReturnsDbSet(new List<Account>());
            _dataContext.Setup(x => x.Database)
                .Returns(_dataFacade.Object);
            _accountRepository = new Data.Repository.AccountRepository(_dataContext.Object, Mock.Of<ILogger<Data.Repository.AccountRepository>>());

            //Act
            await _accountRepository.Add(expectedAccount);

            //Assert 
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
            _dbContextTransaction.Verify(x => x.Commit(), Times.Once);
        }

        [Test]
        public async Task Then_The_Item_Is_Not_Saved_If_It_Already_Exists()
        {
            //Arrange
            var expectedAccount = new Account
            {
                Id = 123,
                Name = "Test"
            };

            _dataContext.Setup(x => x.Accounts.FindAsync(expectedAccount.Id))
                .ReturnsAsync(expectedAccount);
            _dataContext.Setup(x => x.Database)
                .Returns(_dataFacade.Object);
            _accountRepository = new Data.Repository.AccountRepository(_dataContext.Object, Mock.Of<ILogger<Data.Repository.AccountRepository>>());
            

            //Act
            await _accountRepository.Add(expectedAccount);

            //Assert 
            _dataContext.Verify(x => x.SaveChanges(), Times.Never);
        }

    
    }
}