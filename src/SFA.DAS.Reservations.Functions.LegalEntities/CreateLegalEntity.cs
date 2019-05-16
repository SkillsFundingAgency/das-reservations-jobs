using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.NServiceBus;

namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    
    public class HandleManageYourApprenticeshipsEvents
    {
        [FunctionName("HandleManageYourApprenticeshipsEvents")]
        public static async Task Run([NServiceBusTrigger(QueueName = QueueNames.AccountsEndpoint)] KeyValuePair<string,string> message, ILogger log)//, [Inject] IAddedLegalEntityEventHandler handler)
        {
            log.LogInformation($"NServiceBus {message.Key} trigger function executed at: {DateTime.Now}");

            if (message.Key == typeof(LegalEntityAddedEvent).ToString())
            {
                //add to queue
            }
            //if (message.Key == typeof(RemovedLegalEntityEvent).ToString())
            //{
            //    //add to queue
            //}
            //if (message.Key == typeof(SignedAgreementEvent).ToString())
            //{
            //    //add to queue
            //}

            log.LogInformation($"{message.Key} with: {message.Value}");
        }


    }
}
