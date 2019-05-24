﻿using System;
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
    
    public class HandleAddedLegalEntityEvent
    {
        [FunctionName("HandleAddedLegalEntityEvent")]
        public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.LegalEntityAdded)] AddedLegalEntityEvent message, [Inject]IAddAccountLegalEntityHandler handler, ILogger log)
        {
            log.LogInformation($"NServiceBus LegalEntityAdded trigger function executed at: {DateTime.Now}");
            await handler.Handle(message);
        }
    }

    
}
