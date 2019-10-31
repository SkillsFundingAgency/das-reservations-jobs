using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.AccountLegalEntityRepository
{
    public class WhenRemovingAccountLegalEntities
    {
        private AccountLegalEntity _accountLegalEntity;
        private Mock<IReservationsDataContext> _dataContext;
        private Repository.AccountLegalEntityRepository _accountLegalEntityRepository;
        private Mock<IDbContextTransaction> _dbContextTransaction;
        private Mock<DbContext> _dbContext;
        private Mock<DatabaseFacade> _dataFacade;

        [SetUp]
        public void Arrange()
        {
            _accountLegalEntity = new AccountLegalEntity
            {
                Id = Guid.NewGuid(),
                AccountLegalEntityId = 8376234,
                AgreementSigned = false
            };
            _dbContextTransaction = new Mock<IDbContextTransaction>();
            _dbContext = new Mock<DbContext>();
            _dataContext = new Mock<IReservationsDataContext>();
            _dataContext.Setup(x => x.AccountLegalEntities).ReturnsDbSet(new List<AccountLegalEntity>
            {
                _accountLegalEntity
            });
            _dataFacade = new Mock<DatabaseFacade>(_dbContext.Object);
            _dataFacade.Setup(x => x.BeginTransaction()).Returns(_dbContextTransaction.Object);
            _dataContext.Setup(x => x.Database)
                .Returns(_dataFacade.Object);



            _accountLegalEntityRepository = new Repository.AccountLegalEntityRepository(_dataContext.Object, Mock.Of<ILogger<Repository.AccountLegalEntityRepository>>());
        }

        [Test]
        public async Task Then_The_LegalEntity_Is_Updated_If_It_Exists_With_The_Agreement_Signed_Flag()
        {
            //Act
            await _accountLegalEntityRepository.Remove(_accountLegalEntity);

            //Assert
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
        }
    }
}