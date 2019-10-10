using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.Reservations.Application.ProviderPermissions.Service;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Application.UnitTests.ProviderPermission.Service
{
    public class WhenAddingProviderPermission
    {
        private ProviderPermissionService _service;
        private Mock<IProviderPermissionRepository> _repo;

        [SetUp]
        public void Arrange()
        {
            _repo = new Mock<IProviderPermissionRepository>();
            _service = new ProviderPermissionService(_repo.Object);
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
                p.UkPrn.Equals(permissionEvent.Ukprn) &&
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
        public void ThenThrowsArgumentExceptionIfEventIsNull()
        {
            //Act + Assert
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _service.AddProviderPermission(null));

            Assert.AreEqual(nameof(UpdatedPermissionsEvent), exception.ParamName);
        }

        [Test]
        public void ThenThrowsArgumentExceptionIfEventHasNoAccountId()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                0, 11, 12,
                13, 14, Guid.NewGuid(),
                "test@example.com", "Test",
                "Tester", new HashSet<Operation> { Operation.Recruitment }, DateTime.Now);

            //Act + Assert
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _service.AddProviderPermission(permissionEvent));

            Assert.AreEqual(nameof(permissionEvent.AccountId), exception.ParamName);
        }

        [Test]
        public void ThenThrowsArgumentExceptionIfEventHasNoAccountLegalEntityId()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                1, 0, 12,
                13, 14, Guid.NewGuid(),
                "test@example.com", "Test",
                "Tester", new HashSet<Operation> { Operation.Recruitment }, DateTime.Now);

            //Act + Assert
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _service.AddProviderPermission(permissionEvent));

            Assert.AreEqual(nameof(permissionEvent.AccountLegalEntityId), exception.ParamName);
        }

        [Test]
        public void ThenThrowsArgumentExceptionIfEventHasNoUkprn()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                1, 11, 12,
                13, 0, Guid.NewGuid(),
                "test@example.com", "Test",
                "Tester", new HashSet<Operation> { Operation.Recruitment }, DateTime.Now);

            //Act + Assert
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _service.AddProviderPermission(permissionEvent));

            Assert.AreEqual(nameof(permissionEvent.Ukprn), exception.ParamName);
        }



    }
}
