using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.Reservations.Application.ProviderPermissions.Handlers;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Application.UnitTests.ProviderPermission.Handler
{
    public class WhenAddingProviderPermission
    {
        private ProviderPermissionUpdatedHandler _handler;
        private Mock<IProviderPermissionService> _service;

        [SetUp]
        public void Arrange()
        {
            _service = new Mock<IProviderPermissionService>();
            _handler = new ProviderPermissionUpdatedHandler(_service.Object);
        }

        [Test]
        public async Task ThenCallsServiceToAddPermission()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                10, 11, 12, 
                13, 14, Guid.NewGuid(), 
                "test@example.com", "Test", 
                "Tester", new HashSet<Operation>(), DateTime.Now );

            //Act
            await _handler.Handle(permissionEvent);

            //Assert
            _service.Verify(s => s.AddProviderPermission(permissionEvent), Times.Once);
        }
    }
}
