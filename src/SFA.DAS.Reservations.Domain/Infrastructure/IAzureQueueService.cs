using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public interface IAzureQueueService
    {
        Task<IList<QueueMonitor>> GetQueuesToMonitor();
        Task<bool> IsQueueHealthy(string expectedQueueName);
        Task SaveQueueStatus(IList<QueueMonitor> queueMonitors);
    }
}
