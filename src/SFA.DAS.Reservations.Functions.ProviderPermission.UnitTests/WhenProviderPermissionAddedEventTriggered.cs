using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Functions.ProviderPermission.UnitTests;

public class WhenProviderPermissionAddedEventTriggered
{
    [Test]
    public async Task Then_Message_Handler_Called()
    {
        //Arrange
        var handler = new Mock<IProviderPermissionsUpdatedHandler>();
        var message = new UpdatedPermissionsEvent(
            10, 11, 12, 
            13, 14, Guid.NewGuid(), 
            "test@example.com", "Test", 
            "Tester", [], null, DateTime.Now);

        //Act
        await HandleProviderPermissionUpdatedEvent.Run(
            message,
            Mock.Of<ILogger<UpdatedPermissionsEvent>>(),
            handler.Object);

        //Assert
        handler.Verify(s => s.Handle(message), Times.Once);
    }
}