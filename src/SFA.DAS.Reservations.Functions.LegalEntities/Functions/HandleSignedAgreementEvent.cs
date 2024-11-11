using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Functions.LegalEntities.Functions;

public class HandleSignedAgreementEvent(
    ISignedLegalAgreementHandler handler, ILogger<SignedAgreementEvent> log) : IHandleMessages<SignedAgreementEvent>
{
    public async Task Handle(SignedAgreementEvent message, IMessageHandlerContext context)
    {
        log.LogInformation($"NServiceBus HandleSignedAgreementEvent trigger function executed at: {DateTime.Now} for account:{message.AccountId}-legal entity id:{message.LegalEntityId} - {message.OrganisationName}");
        await handler.Handle(message);
        log.LogInformation($"NServiceBus HandleSignedAgreementEvent trigger function finished");
    }
}