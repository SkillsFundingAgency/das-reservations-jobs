using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Functions.LegalEntities.Functions;

public class HandleRemovedLegalEntityEvent(IRemoveLegalEntityHandler handler,
    ILogger<RemovedLegalEntityEvent> log) : IHandleMessages<RemovedLegalEntityEvent>
{
    public async Task Handle(RemovedLegalEntityEvent message, IMessageHandlerContext context)
    {
        log.LogInformation($"NServiceBus RemovedLegalEntity trigger function executed at: {DateTime.Now} for ${message.AccountLegalEntityId}:${message.OrganisationName}");
        await handler.Handle(message);
        log.LogInformation($"NServiceBus RemovedLegalEntity trigger function finished at: {DateTime.Now} for ${message.AccountLegalEntityId}:${message.OrganisationName}");
    }
}