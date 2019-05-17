using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    public class RemoveAccountLegalEntity
    {
        [FunctionName("RemoveAccountLegalEntity")]
        public static async Task Run([QueueTrigger(QueueNames.RemovedLegalEntity)]AccountLegalEntityRemovedEvent accountLegalEntityRemoved, ILogger log, [Inject]IRemoveLegalEntityHandler handler)
        {
            log.LogInformation("C# Queue trigger function processed for RemoveAccountLegalEntity");
            await handler.Handle(accountLegalEntityRemoved);
        }
    }
}