using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.Reservations.Application.ProviderPermissions.Handlers;
using SFA.DAS.Reservations.Application.UnitTests.Customisations;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.ProviderPermission.Handler
{
    public class WhenAddingProviderPermission
    {
        [Test, MoqAutoData]
        public async Task And_Validation_Error_Then_No_Further_Processing(
            [ArrangeUpdatedPermissionsEvent(Operation = Operation.CreateCohort)] UpdatedPermissionsEvent updatedEvent,
            [Frozen] Mock<IUpdatedPermissionsEventValidator> mockValidator,
            [Frozen] Mock<IProviderPermissionService> mockPermissionsService,
            ProviderPermissionsUpdatedHandler handler)
        {
            mockValidator
                .Setup(validator => validator.Validate(updatedEvent))
                .Returns(false);

            await handler.Handle(updatedEvent);

            mockPermissionsService.Verify(service => service.AddProviderPermission(It.IsAny<UpdatedPermissionsEvent>()), 
                Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_Calls_Service(
            [ArrangeUpdatedPermissionsEvent(Operation = Operation.CreateCohort)] UpdatedPermissionsEvent updatedEvent,
            [Frozen] Mock<IUpdatedPermissionsEventValidator> mockValidator,
            [Frozen] Mock<IProviderPermissionService> mockPermissionsService,
            ProviderPermissionsUpdatedHandler handler)
        {
            await handler.Handle(updatedEvent);

            //Assert
            mockPermissionsService.Verify(s => s.AddProviderPermission(updatedEvent), 
                Times.Once);
        }
    }
}
