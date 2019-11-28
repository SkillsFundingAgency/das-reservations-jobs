using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    public class HandleAccountNameUpdatedEvent
    {
        [FunctionName("HandleAccountNameUpdatedEvent")]
        public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.AccountUpdated)] ChangedAccountNameEvent message, [Inject]IAccountNameUpdatedHandler handler, [Inject]ILogger<ChangedAccountNameEvent> log)
        {
            log.LogInformation($"NServiceBus AccountCreated trigger function executed at: {DateTime.Now} for ${message.AccountId}:${message.CurrentName}");
            await handler.Handle(message);
            log.LogInformation($"NServiceBus AccountCreated trigger function finished at: {DateTime.Now} for ${message.AccountId}:${message.CurrentName}");
        }
    }
}