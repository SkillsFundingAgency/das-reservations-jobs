using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.Reservations.Application.ProviderPermissions.Service;
using SFA.DAS.Reservations.Application.UnitTests.Extensions;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Application.UnitTests.ProviderPermission.Service
{
    public class WhenAddingProviderPermission
    {
        private ProviderPermissionService _service;
        private Mock<IProviderPermissionRepository> _repo;
        private Mock<ILogger<ProviderPermissionService>> _logger;

        [SetUp]
        public void Arrange()
        {
            _repo = new Mock<IProviderPermissionRepository>();
            _logger = new Mock<ILogger<ProviderPermissionService>>();
            _service = new ProviderPermissionService(_repo.Object, _logger.Object);
        }

        [Test]
        public async Task ThenCallsRepositoryToAddPermissionToDatabase()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                10, 11, 12, 
                13, 14, Guid.NewGuid(), 
                "test@example.com", "Test", 
                "Tester", new HashSet<Operation>{Operation.CreateCohort}, DateTime.Now );

            //Act
            await _service.AddProviderPermission(permissionEvent);

            //Assert
            _repo.Verify(s => s.Add(It.Is<Domain.Entities.ProviderPermission>(p => 
                p.AccountId.Equals(permissionEvent.AccountId) &&
                p.AccountLegalEntityId.Equals(permissionEvent.AccountLegalEntityId) &&
                p.ProviderId.Equals(permissionEvent.Ukprn) &&
                p.CanCreateCohort)), Times.Once);
        }

        [Test]
        public async Task ThenSetsCanCreateCohortToFalseIfNoPermissionFound()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                10, 11, 12,
                13, 14, Guid.NewGuid(),
                "test@example.com", "Test",
                "Tester", new HashSet<Operation>{ Operation.Recruitment }, DateTime.Now);

            //Act
            await _service.AddProviderPermission(permissionEvent);

            //Assert
            _repo.Verify(s => s.Add(It.Is<Domain.Entities.ProviderPermission>(p =>
                !p.CanCreateCohort)), Times.Once);
        }


        [Test]
        public async Task ThenSetsCanCreateCohortToFalseIfPermissionIsNull()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                10, 11, 12,
                13, 14, Guid.NewGuid(),
                "test@example.com", "Test",
                "Tester", null, DateTime.Now);

            //Act
            await _service.AddProviderPermission(permissionEvent);

            //Assert
            _repo.Verify(s => s.Add(It.Is<Domain.Entities.ProviderPermission>(p =>
                !p.CanCreateCohort)), Times.Once);
        }

        [Test]
        public async Task ThenLogsWarningIfEventIsNull()
        {
            //Act
            await _service.AddProviderPermission(null);

            //Assert
            _logger.VerifyLog(LogLevel.Warning);
        }

        [Test]
        public async Task ThenLogsWarningIfEventHasNoAccountId()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                0, 11, 12,
                13, 14, Guid.NewGuid(),
                "test@example.com", "Test",
                "Tester", new HashSet<Operation> { Operation.Recruitment }, DateTime.Now);

            //Act
            await _service.AddProviderPermission(permissionEvent);

            //Assert
            _logger.VerifyLog(LogLevel.Warning);
        }

        [Test]
        public async Task ThenLogsWarningIfEventHasNoAccountLegalEntityId()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                1, 0, 12,
                13, 14, Guid.NewGuid(),
                "test@example.com", "Test",
                "Tester", new HashSet<Operation> { Operation.Recruitment }, DateTime.Now);

            //Act
            await _service.AddProviderPermission(permissionEvent);

            //Assert
            _logger.VerifyLog(LogLevel.Warning);
        }

        [Test]
        public async Task ThenLogsWarningIfEventHasNoUkprn()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                1, 11, 12,
                13, 0, Guid.NewGuid(),
                "test@example.com", "Test",
                "Tester", new HashSet<Operation> { Operation.Recruitment }, DateTime.Now);

            //Act
            await _service.AddProviderPermission(permissionEvent);

            //Assert
            _logger.VerifyLog(LogLevel.Warning);
        }



    }
}
