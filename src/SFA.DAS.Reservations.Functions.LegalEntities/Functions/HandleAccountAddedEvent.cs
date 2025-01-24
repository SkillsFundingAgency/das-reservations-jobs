using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Functions.LegalEntities.Functions;

public class HandleAccountAddedEvent(IAddAccountHandler handler,
    ILogger<CreatedAccountEvent> log) : IHandleMessages<CreatedAccountEvent>
{
    public async Task Handle(CreatedAccountEvent message, IMessageHandlerContext context)
    {
        log.LogInformation($"NServiceBus AccountCreated trigger function executed at: {DateTime.Now} for ${message.AccountId}:${message.Name}");
        await handler.Handle(message);
        log.LogInformation($"NServiceBus AccountCreated trigger function finished at: {DateTime.Now} for ${message.AccountId}:${message.Name}");
    }
}