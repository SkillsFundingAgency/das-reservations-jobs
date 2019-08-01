using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    public class HandleLevyAddedToAccountEvent
    {
        [FunctionName("LevyAddedToAccount")]
        public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.LevyAddedToAccount)]LevyAddedToAccount message, [Inject] ILevyAddedToAccountHandler handler,[Inject] ILogger<LevyAddedToAccount> log)
        {
            log.LogInformation($"NServiceBus LevyAddedToAccount trigger function executed at: {DateTime.Now} for ${nameof(message.AccountId)}:${message.AccountId}");
            await handler.Handle(message);
        }
    }
}
