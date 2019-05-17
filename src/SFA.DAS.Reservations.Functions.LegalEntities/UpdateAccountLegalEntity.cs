using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    public class UpdateAccountLegalEntity
    {
        [FunctionName("UpdateAccountLegalEntity")]
        public static async Task Run([QueueTrigger(QueueNames.LegalEntityAdded)]SignedAgreementEvent signedAgreementEvent, ILogger log, [Inject]ISignedLegalAgreementHandler handler)
        {
            log.LogInformation("C# Queue trigger function processed UpdateAccountLegalEntity");
            await handler.Handle(signedAgreementEvent);
        }
    }
}