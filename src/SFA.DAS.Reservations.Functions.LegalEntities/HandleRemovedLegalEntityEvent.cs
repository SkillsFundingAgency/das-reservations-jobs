﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    public class HandleRemovedLegalEntityEvent
    {

        public class HandleSignedAgreementEvent(
            IRemoveLegalEntityHandler handler, ILogger<RemovedLegalEntityEvent> log) : IHandleMessages<RemovedLegalEntityEvent>
        {
            public async Task Handle(RemovedLegalEntityEvent message, IMessageHandlerContext context)
            {
                log.LogInformation($"NServiceBus RemovedLegalEntity trigger function executed at: {DateTime.Now} for ${message.AccountLegalEntityId}:${message.OrganisationName}");
                await handler.Handle(message);
                log.LogInformation($"NServiceBus RemovedLegalEntity trigger function finished at: {DateTime.Now} for ${message.AccountLegalEntityId}:${message.OrganisationName}");
            }
        }



        //[FunctionName("HandleRemovedLegalEntityEvent")]
        //public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.RemovedLegalEntity)] RemovedLegalEntityEvent message, [Inject]IRemoveLegalEntityHandler handler, [Inject]ILogger<RemovedLegalEntityEvent> log)
        //{
        //    log.LogInformation($"NServiceBus RemovedLegalEntity trigger function executed at: {DateTime.Now} for ${message.AccountLegalEntityId}:${message.OrganisationName}");
        //    await handler.Handle(message);
        //    log.LogInformation($"NServiceBus RemovedLegalEntity trigger function finished at: {DateTime.Now} for ${message.AccountLegalEntityId}:${message.OrganisationName}");
        //}
    }
}