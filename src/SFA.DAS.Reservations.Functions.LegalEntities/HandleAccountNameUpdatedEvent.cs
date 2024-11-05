using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    public class HandleAccountNameUpdatedEvent(
        IAccountNameUpdatedHandler handler, ILogger<ChangedAccountNameEvent> log) : IHandleMessages<ChangedAccountNameEvent>
    {
        public async Task Handle(ChangedAccountNameEvent message, IMessageHandlerContext context)
        {
            log.LogInformation($"NServiceBus AccountCreated trigger function executed at: {DateTime.Now} for ${message.AccountId}:${message.CurrentName}");
            await handler.Handle(message);
            log.LogInformation($"NServiceBus AccountCreated trigger function finished at: {DateTime.Now} for ${message.AccountId}:${message.CurrentName}");
        }
    }



    //[FunctionName("HandleAccountNameUpdatedEvent")]
    //    public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.AccountUpdated)] ChangedAccountNameEvent message, [Inject]IAccountNameUpdatedHandler handler, [Inject]ILogger<ChangedAccountNameEvent> log)
    //    {
    //        log.LogInformation($"NServiceBus AccountCreated trigger function executed at: {DateTime.Now} for ${message.AccountId}:${message.CurrentName}");
    //        await handler.Handle(message);
    //        log.LogInformation($"NServiceBus AccountCreated trigger function finished at: {DateTime.Now} for ${message.AccountId}:${message.CurrentName}");
    //    }

}