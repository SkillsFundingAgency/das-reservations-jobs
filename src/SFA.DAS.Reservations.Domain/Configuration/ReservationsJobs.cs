using System.Collections;
using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Domain.Configuration
{
    public class ReservationsJobs
    {
        public string ApprenticeshipBaseUrl { get; set; }
        public string ConnectionString { get; set; }
        public string NServiceBusConnectionString { get; set; }
        public string AzureWebJobsStorage { get; set; }
        public virtual string QueueMonitorItems { get; set; }
        public string SlackChannelUrl { get; set; }
        public string SlackSecret { get; set; }
    }
}
