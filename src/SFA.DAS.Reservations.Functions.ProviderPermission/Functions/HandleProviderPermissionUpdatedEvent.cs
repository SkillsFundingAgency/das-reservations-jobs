using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Functions.ProviderPermission.Functions;

public class HandleProviderPermissionUpdatedEvent(
    IProviderPermissionsUpdatedHandler _providerPermissionsUpdatedHandler,
    ILogger<UpdatedPermissionsEvent> _logger)
    : IHandleMessages<UpdatedPermissionsEvent>
{
    public async Task Handle(UpdatedPermissionsEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation($"NServiceBus AddedAccountProvider trigger function executed at: {DateTime.Now}");
        await _providerPermissionsUpdatedHandler.Handle(message);
    }
}