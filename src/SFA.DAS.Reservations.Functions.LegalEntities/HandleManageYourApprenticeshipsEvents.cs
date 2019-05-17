﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.NServiceBus;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    
    public class HandleManageYourApprenticeshipsEvents
    {
        [FunctionName("HandleManageYourApprenticeshipsEvents")]
        public static async Task Run([NServiceBusTrigger(QueueName = QueueNames.AccountsEndpoint)] KeyValuePair<string,string> message, IAzureQueueService queueService, ILogger log)//, [Inject] IAddedLegalEntityEventHandler handler)
        {
            log.LogInformation($"NServiceBus {message.Key} trigger function executed at: {DateTime.Now}");

            if (message.Key.Equals(typeof(AccountLegalEntityAddedEvent).ToString()))
            {
                var entityAddedEvent = JsonConvert.DeserializeObject<AccountLegalEntityAddedEvent>(message.Value);

                await queueService.SendMessage(entityAddedEvent, QueueNames.LegalEntityAdded);
            }
            
            if (message.Key.Equals(typeof(AccountLegalEntityRemovedEvent).ToString()))
            {
                var entityAddedEvent = JsonConvert.DeserializeObject<AccountLegalEntityRemovedEvent>(message.Value);

                await queueService.SendMessage(entityAddedEvent, QueueNames.RemovedLegalEntity);
            }
            
            if (message.Key.Equals(typeof(SignedAgreementEvent).ToString()))
            {
                var entityAddedEvent = JsonConvert.DeserializeObject<SignedAgreementEvent>(message.Value);

                await queueService.SendMessage(entityAddedEvent, QueueNames.SignedAgreement);
            }

            log.LogInformation($"{message.Key} with: {message.Value}");
        }
    }
}
