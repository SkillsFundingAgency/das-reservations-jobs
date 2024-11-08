using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Functions.LegalEntities;

public class HandleAddedLegalEntityEvent(IAddAccountLegalEntityHandler handler, 
    ILogger<AddedLegalEntityEvent> log) : IHandleMessages<AddedLegalEntityEvent>
{
    public async Task Handle(AddedLegalEntityEvent message, IMessageHandlerContext context)
    {
        log.LogInformation($"NServiceBus LegalEntityAdded trigger function executed at: {DateTime.Now} for ${message.AccountLegalEntityId}:${message.OrganisationName}");
        await handler.Handle(message);
        log.LogInformation($"NServiceBus LegalEntityAdded trigger function finished at: {DateTime.Now} for ${message.AccountLegalEntityId}:${message.OrganisationName}");
    }
}