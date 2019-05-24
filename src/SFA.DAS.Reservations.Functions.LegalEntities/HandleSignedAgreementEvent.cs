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
    public class HandleSignedAgreementEvent
    {
        [FunctionName("HandleSignedAgreementEvent")]
        public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.SignedAgreement)] SignedAgreementEvent message, [Inject]ISignedLegalAgreementHandler handler, ILogger log)
        {
            log.LogInformation("C# Queue trigger function processed UpdateAccountLegalEntity");
            await handler.Handle(message);
        }
    }
}