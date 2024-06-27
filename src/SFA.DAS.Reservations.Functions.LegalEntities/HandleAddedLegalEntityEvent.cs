using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    
    public class HandleAddedLegalEntityEvent
    {
        [FunctionName("HandleAddedLegalEntityEvent")]
        public static async Task Run([NServiceBusTrigger(Endpoint = QueueNames.LegalEntityAdded)] AddedLegalEntityEvent message, [Inject]IAddAccountLegalEntityHandler handler, [Inject]ILogger<AddedLegalEntityEvent> log)
        {
            log.LogInformation($"NServiceBus LegalEntityAdded trigger function executed at: {DateTime.Now} for ${message.AccountLegalEntityId}:${message.OrganisationName}");
            await handler.Handle(message);
            log.LogInformation($"NServiceBus LegalEntityAdded trigger function finished at: {DateTime.Now} for ${message.AccountLegalEntityId}:${message.OrganisationName}");
        }
    }

    
}
