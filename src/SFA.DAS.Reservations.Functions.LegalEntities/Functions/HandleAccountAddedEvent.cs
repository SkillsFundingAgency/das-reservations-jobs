using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.LegalEntities.Functions;

public class HandleAccountAddedEvent
{
    [FunctionName("HandleAccountAddedEvent")]
    public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.AccountCreated)] CreatedAccountEvent message, [Inject] IAddAccountHandler handler, [Inject] ILogger<CreatedAccountEvent> log)
    {
        log.LogInformation("NServiceBus AccountCreated trigger function executed for {AccountId}:{Name}", message.AccountId, message.Name);

        await handler.Handle(message);

        log.LogInformation("NServiceBus AccountCreated trigger function finished for {AccountId}:{Name}", message.AccountId, message.Name);
    }
}