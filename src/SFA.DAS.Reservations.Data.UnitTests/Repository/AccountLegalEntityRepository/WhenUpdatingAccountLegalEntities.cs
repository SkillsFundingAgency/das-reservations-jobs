using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.AccountLegalEntityRepository
{
    public class WhenUpdatingAccountLegalEntities
    {
        private AccountLegalEntity _dbAccountLegalEntity;
        private AccountLegalEntity _updatedAccountLegalEntity;
        private Mock<IReservationsDataContext> _dataContext;
        private Data.Repository.AccountLegalEntityRepository _accountLegalEntityRepository;
        private Mock<IDbContextTransaction> _dbContextTransaction;
        private Mock<DbContext> _dbContext;
        private Mock<DatabaseFacade> _dataFacade;

        [SetUp]
        public void Arrange()
        {
            _dbAccountLegalEntity = new AccountLegalEntity
            {
                Id = Guid.NewGuid(),
                AccountId = 8376234,
                AgreementSigned = false,
                LegalEntityId = 4,
                AgreementType = AgreementType.Levy
            };

            _updatedAccountLegalEntity = new AccountLegalEntity
            {
                Id = Guid.NewGuid(),
                AccountId = 8376234,
                AgreementSigned = false,
                LegalEntityId = 4,
                AgreementType = AgreementType.NoneLevyExpressionOfInterest
            };

            _dbContextTransaction = new Mock<IDbContextTransaction>();
            _dbContext = new Mock<DbContext>();
            _dataContext = new Mock<IReservationsDataContext>();
            _dataFacade = new Mock<DatabaseFacade>(_dbContext.Object);
            _dataFacade.Setup(x => x.BeginTransaction()).Returns(_dbContextTransaction.Object);
            _dataContext.Setup(x => x.Database)
                .Returns(_dataFacade.Object);

            _dataContext.Setup(x => x.AccountLegalEntities).ReturnsDbSet(new List<AccountLegalEntity>
            {
                _dbAccountLegalEntity
            });


            _accountLegalEntityRepository = new Data.Repository.AccountLegalEntityRepository(_dataContext.Object);
        }

        [Test]
        public async Task Then_The_LegalEntity_Is_Updated_If_It_Exists_With_The_Agreement_Signed_Flag()
        {
            //Act
            await _accountLegalEntityRepository.UpdateAgreementStatus(
                _updatedAccountLegalEntity);
            
            //Assert
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Test]
        public async Task ThenWillMarkTheAgreementAsSigned()
        {
            //Act
            await _accountLegalEntityRepository.UpdateAgreementStatus(
                _updatedAccountLegalEntity);
            
            //Assert
            Assert.IsTrue(_dbAccountLegalEntity.AgreementSigned);
        }

        [Test]
        public async Task ThenWillUpdateTheAgreementType()
        {
            //Act
            await _accountLegalEntityRepository.UpdateAgreementStatus(_updatedAccountLegalEntity);
            
            //Assert
            Assert.AreEqual(AgreementType.NoneLevyExpressionOfInterest, _dbAccountLegalEntity.AgreementType);
        }

        [Test]
        public void Then_If_The_Entity_Does_Not_Exist_An_Exception_Is_Thrown()
        {
            Assert.ThrowsAsync<DbUpdateException>(() => _accountLegalEntityRepository.UpdateAgreementStatus(new AccountLegalEntity{AccountId = 54, LegalEntityId = 2}));

            //Assert
            _dataContext.Verify(x => x.SaveChanges(), Times.Never);
        }

        [Test,MoqAutoData]
        public async Task AndUpdatingAccountToLevy_ThenEntityIsUpdatedIfItExists(
            AccountLegalEntity accountLegalEntity)
        {
            _dataContext.Setup(x => x.AccountLegalEntities).ReturnsDbSet(new List<AccountLegalEntity>
            {
                accountLegalEntity
            });

            //Act
            await _accountLegalEntityRepository.UpdateAccountLegalEntitiesToLevy(accountLegalEntity);

            //Assert
            _dataContext.Verify(x => x.SaveChanges(),Times.Once);
        }

        [Test, MoqAutoData]
        public void AndUpdatingAccountToLevy_ThenExceptionThrownIfEntityDoesExist(
            AccountLegalEntity accountLegalEntity)
        {
            //Act
            Assert.ThrowsAsync<DbUpdateException>( () => _accountLegalEntityRepository.UpdateAccountLegalEntitiesToLevy(accountLegalEntity));

            //Assert
            _dataContext.Verify(x => x.SaveChanges(), Times.Never);
        }
    }
}
