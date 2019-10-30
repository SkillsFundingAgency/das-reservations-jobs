using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.ProviderPermissionsRepository
{
    public class WhenGettingAllPermissions
    {
        private Data.Repository.ProviderPermissionsRepository _permissionsRepository;
        private Mock<IReservationsDataContext> _dataContext;
        private Mock<DatabaseFacade> _dataFacade;
        private Mock<DbContext> _dbContext;
        private Mock<IDbContextTransaction> _dbContextTransaction;
        private IEnumerable<ProviderPermission> _expectedPermissions;

        [SetUp]
        public void Arrange()
        {
            _expectedPermissions = new List<ProviderPermission>
            {
                new ProviderPermission { AccountId = 1, AccountLegalEntityId = 1, ProviderId = 1, CanCreateCohort = true},
                new ProviderPermission { AccountId = 1, AccountLegalEntityId = 2, ProviderId = 1, CanCreateCohort = true },
                new ProviderPermission { AccountId = 1, AccountLegalEntityId = 3, ProviderId = 1, CanCreateCohort = false }
            };

            _dbContextTransaction = new Mock<IDbContextTransaction>();
            _dbContext = new Mock<DbContext>();
            _dataContext = new Mock<IReservationsDataContext>();
            _dataFacade = new Mock<DatabaseFacade>(_dbContext.Object);
            _dataFacade.Setup(x => x.BeginTransaction()).Returns(_dbContextTransaction.Object);

            _dataContext.Setup(x => x.ProviderPermissions).ReturnsDbSet(_expectedPermissions);
            _dataContext.Setup(x => x.Database).Returns(_dataFacade.Object);

            _permissionsRepository = new Data.Repository.ProviderPermissionsRepository(_dataContext.Object);
        }

        [Test]
        public void ThenReturnsAllReservations()
        {
            //Act
            var reservations = _permissionsRepository.GetAllWithCreateCohortPermission();

            //Assert 
            reservations.Should().BeEquivalentTo(_expectedPermissions.Where(c=>c.CanCreateCohort));
        }
    }
}
