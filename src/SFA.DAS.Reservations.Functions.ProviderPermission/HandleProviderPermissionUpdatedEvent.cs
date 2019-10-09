
using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Functions.ProviderPermission
{
    
    public class HandleProviderPermissionUpdatedEvent
    {
        [FunctionName("HandleProviderPermissionUpdatedEvent")]
        public static async Task Run([NServiceBusTrigger(EndPoint = "")] UpdatedPermissionsEvent updateEvent, ILogger<UpdatedPermissionsEvent> logger, IProviderPermissionUpdatedHandler providerPermissionUpdatedHandler)
        {
            logger.LogInformation($"NServiceBus AddedAccountProvider trigger function executed at: {DateTime.Now}");
            await providerPermissionUpdatedHandler.Handle(updateEvent);
        }
    }
}
