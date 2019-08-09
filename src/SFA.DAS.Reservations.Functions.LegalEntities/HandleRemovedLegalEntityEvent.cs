using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    public class HandleRemovedLegalEntityEvent
    {
        [FunctionName("HandleRemovedLegalEntityEvent")]
        public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.RemovedLegalEntity)] RemovedLegalEntityEvent message, [Inject]IRemoveLegalEntityHandler handler, [Inject]ILogger<RemovedLegalEntityEvent> log)
        {
            log.LogInformation($"NServiceBus RemovedLegalEntity trigger function executed at: {DateTime.Now} for ${message.AccountLegalEntityId}:${message.OrganisationName}");
            await handler.Handle(message);
            log.LogInformation($"NServiceBus RemovedLegalEntity trigger function finished at: {DateTime.Now} for ${message.AccountLegalEntityId}:${message.OrganisationName}");
        }
    }
}