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
    public class HandleSignedAgreementEvent
    {
        [FunctionName("HandleSignedAgreementEvent")]
        public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.SignedAgreement)] SignedAgreementEvent message, [Inject]ISignedLegalAgreementHandler handler, [Inject]ILogger<SignedAgreementEvent> log)
        {
            log.LogInformation($"NServiceBus HandleSignedAgreementEvent trigger function executed at: {DateTime.Now} for account:{message.AccountId}-legal entity id:{message.LegalEntityId} - {message.OrganisationName}");
            await handler.Handle(message);
            log.LogInformation($"NServiceBus HandleSignedAgreementEvent trigger function finished");
        }
    }
}