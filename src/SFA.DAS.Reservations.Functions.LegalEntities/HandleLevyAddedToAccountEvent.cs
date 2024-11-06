using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    public class HandleLevyAddedToAccountEvent(ILevyAddedToAccountHandler handler,
        ILogger<LevyAddedToAccount> log) : IHandleMessages<LevyAddedToAccount>
    {
        public async Task Handle(LevyAddedToAccount message, IMessageHandlerContext context)
        {
            log.LogInformation($"NServiceBus LevyAddedToAccount trigger function started execution at: {DateTime.Now} for ${nameof(message.AccountId)}:${message.AccountId}");
            await handler.Handle(message);
            log.LogInformation($"NServiceBus LevyAddedToAccount trigger function finished execution at: {DateTime.Now} for ${nameof(message.AccountId)}:${message.AccountId}");
        }
    }

    //[FunctionName("LevyAddedToAccount")]
    //public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.LevyAddedToAccount)]LevyAddedToAccount message, [Inject] ILevyAddedToAccountHandler handler,[Inject] ILogger<LevyAddedToAccount> log)
    //{
    //    log.LogInformation($"NServiceBus LevyAddedToAccount trigger function started execution at: {DateTime.Now} for ${nameof(message.AccountId)}:${message.AccountId}");
    //    await handler.Handle(message);
    //    log.LogInformation($"NServiceBus LevyAddedToAccount trigger function finished execution at: {DateTime.Now} for ${nameof(message.AccountId)}:${message.AccountId}");
    //}
}
