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
        public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.AccountsEndpoint, 
            EndPointConfigurationTypes = "SFA.DAS.EmployerAccounts.Messages.Events.AddedLegalEntityEvent,SFA.DAS.EmployerAccounts.Messages.Events.RemovedLegalEntityEvent,SFA.DAS.EmployerAccounts.Messages.Events.SignedAgreementEvent")] KeyValuePair<string,string> message, [Inject]IAzureQueueService queueService, ILogger log)
        {
            log.LogInformation($"NServiceBus {message.Key} trigger function executed at: {DateTime.Now}");

            switch (message.Key)
            {
                case "SFA.DAS.EmployerAccounts.Messages.Events.AddedLegalEntityEvent":
                    var accountLegalEntityAddedEvent = JsonConvert.DeserializeObject<AccountLegalEntityAddedEvent>(message.Value);
                    await queueService.SendMessage(accountLegalEntityAddedEvent, QueueNames.LegalEntityAdded);
                    break;
                case "SFA.DAS.EmployerAccounts.Messages.Events.RemovedLegalEntityEvent":
                    var accountLegalEntityRemovedEvent = JsonConvert.DeserializeObject<AccountLegalEntityRemovedEvent>(message.Value);
                    await queueService.SendMessage(accountLegalEntityRemovedEvent, QueueNames.RemovedLegalEntity);
                    break;
                case "SFA.DAS.EmployerAccounts.Messages.Events.SignedAgreementEvent":
                    var signedAgreementEvent = JsonConvert.DeserializeObject<SignedAgreementEvent>(message.Value);
                    await queueService.SendMessage(signedAgreementEvent, QueueNames.SignedAgreement);
                    break;
                default:
                    log.LogInformation($"No mapping to process message of type:{message.Key}");
                    break;
            }
        }
    }
}
