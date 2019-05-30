using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;
using SFA.DAS.Reservations.Infrastructure.NServiceBus;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    public class HandleRemovedLegalEntityEvent
    {
        [FunctionName("HandleRemovedLegalEntityEvent")]
        public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.RemovedLegalEntity)] RemovedLegalEntityEvent message, [Inject]IRemoveLegalEntityHandler handler, ILogger log)
        {
            log.LogInformation($"NServiceBus RemovedLegalEntity trigger function executed at: {DateTime.Now}");
            await handler.Handle(message);
        }
    }
}