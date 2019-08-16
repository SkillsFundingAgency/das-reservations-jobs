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
        public virtual IList<QueueMonitor> QueueMonitorItems { get; set; }
    }
}
