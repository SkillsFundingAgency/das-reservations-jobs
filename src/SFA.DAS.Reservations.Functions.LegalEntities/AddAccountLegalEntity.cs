using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    public class AddAccountLegalEntity
    {
        [FunctionName("AddAccountLegalEntity")]
        public static async Task Run([QueueTrigger(QueueNames.LegalEntityAdded)]AccountLegalEntityAddedEvent accountLegalEntityAdded, ILogger log, [Inject]IAddAccountLegalEntityHandler handler)
        {
            log.LogInformation("C# Queue trigger function processed AddAccountLegalEntity");
            await handler.Handle(accountLegalEntityAdded);
        }
    }
}