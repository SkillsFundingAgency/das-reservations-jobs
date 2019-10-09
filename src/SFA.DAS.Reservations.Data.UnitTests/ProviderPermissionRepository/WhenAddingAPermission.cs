using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.ProviderPermissionRepository
{
    public class WhenAddingAPermission
    {
        private Repository.ProviderPermissionRepository _providerPermissionRepository;
        private Mock<IReservationsDataContext> _dataContext;
        private Mock<DatabaseFacade> _dataFacade;
        private Mock<DbContext> _dbContext;
        private Mock<IDbContextTransaction> _dbContextTransaction;

        [SetUp]
        public void Arrange()
        {
            var expectedPermission = new ProviderPermission
            {
                AccountId = 1,
                AccountLegalEntityId = 2,
                UkPrn = 3,
                CanCreateCohort = true
            };

            _dbContextTransaction = new Mock<IDbContextTransaction>();
            _dbContext = new Mock<DbContext>();
            _dataContext = new Mock<IReservationsDataContext>();
            _dataFacade = new Mock<DatabaseFacade>(_dbContext.Object);
            _dataFacade.Setup(x => x.BeginTransaction()).Returns(_dbContextTransaction.Object);
            _dataContext.Setup(x => x.ProviderPermissions).ReturnsDbSet(new List<ProviderPermission>
            {
                expectedPermission
            });
            
            _dataContext.Setup(x => x.ProviderPermissions.AddAsync(It.IsAny<ProviderPermission>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityEntry<ProviderPermission>) null);
            
            _dataContext.Setup(x => x.ProviderPermissions.FindAsync(expectedPermission.AccountId, expectedPermission.AccountLegalEntityId, expectedPermission.UkPrn))
                .ReturnsAsync(expectedPermission);

            _dataContext.Setup(x => x.Database)
                .Returns(_dataFacade.Object);

            _providerPermissionRepository = new Repository.ProviderPermissionRepository(_dataContext.Object);
        }

        [Test]
        public async Task Then_The_Permission_Is_Added_To_The_Repository()
        {
            //Arrange
            var expectedPermission = new ProviderPermission
            {
               AccountId = 2,
               AccountLegalEntityId = 3,
               UkPrn = 4,
               CanCreateCohort = true
            };

            //Act
            await _providerPermissionRepository.Add(expectedPermission);

            //Assert 
            _dataContext.Verify(x=>x.ProviderPermissions.AddAsync(It.Is<ProviderPermission>(
                c => c.AccountId.Equals(expectedPermission.AccountId) &&
                     c.AccountLegalEntityId.Equals(expectedPermission.AccountLegalEntityId) &&
                     c.UkPrn.Equals(expectedPermission.UkPrn) &&
                     c.CanCreateCohort.Equals(expectedPermission.CanCreateCohort)
                     ),It.IsAny<CancellationToken>()),Times.Once);
            
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);

        }

        [Test]
        public async Task Then_If_The_Permission_Exists_It_Is_Updated()
        {
            //Arrange
            var expectedPermission = new ProviderPermission
            {
                AccountId = 1,
                AccountLegalEntityId = 2,
                UkPrn = 3,
                CanCreateCohort = true
            };

            //Act
            await _providerPermissionRepository.Add(expectedPermission);

            //Assert
            _dataContext.Verify(x => x.ProviderPermissions.AddAsync(It.IsAny<ProviderPermission>(), It.IsAny<CancellationToken>()), Times.Never);
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
        }
    }
}
