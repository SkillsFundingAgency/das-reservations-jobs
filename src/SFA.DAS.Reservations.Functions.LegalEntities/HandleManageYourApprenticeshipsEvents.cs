using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;
using SFA.DAS.Reservations.Infrastructure.NServiceBus;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    
    public class HandleManageYourApprenticeshipsEvents
    {
        [FunctionName("HandleManageYourApprenticeshipsEvents")]
        public static async Task Run([NServiceBusTrigger(QueueName = QueueNames.AccountsEndpoint)] KeyValuePair<string,string> message, [Inject]IAzureQueueService queueService, ILogger log)
        {
            log.LogInformation($"NServiceBus {message.Key} trigger function executed at: {DateTime.Now}");

            if (message.Key.Equals("SFA.DAS.EmployerAccounts.Messages.Events.AddedLegalEntityEvent"))
            {
                var accountLegalEntityAddedEvent = JsonConvert.DeserializeObject<AccountLegalEntityAddedEvent>(message.Value);

                await queueService.SendMessage(accountLegalEntityAddedEvent, QueueNames.LegalEntityAdded);
            }
            
            if (message.Key.Equals("SFA.DAS.EmployerAccounts.Messages.Events.RemovedLegalEntityEvent"))
            {
                var accountLegalEntityRemovedEvent = JsonConvert.DeserializeObject<AccountLegalEntityRemovedEvent>(message.Value);

                await queueService.SendMessage(accountLegalEntityRemovedEvent, QueueNames.RemovedLegalEntity);
            }
            
            if (message.Key.Equals("SFA.DAS.EmployerAccounts.Messages.Events.SignedAgreementEvent"))
            {
                var signedAgreementEvent = JsonConvert.DeserializeObject<SignedAgreementEvent>(message.Value);

                await queueService.SendMessage(signedAgreementEvent, QueueNames.SignedAgreement);
            }

            log.LogInformation($"{message.Key} with: {message.Value}");
        }
    }
}
