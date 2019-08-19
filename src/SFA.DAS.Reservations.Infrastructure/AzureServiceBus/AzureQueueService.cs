using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.AzureServiceBus
{
    public class AzureQueueService : IAzureQueueService
    {
        private readonly ICacheStorageService _memoryCache;
        private readonly JobEnvironment _jobEnvironment;
        private readonly ReservationsJobs _configuration;

        public AzureQueueService(IOptions<ReservationsJobs> options, ICacheStorageService memoryCache, JobEnvironment jobEnvironment)
        {
            _memoryCache = memoryCache;
            _jobEnvironment = jobEnvironment;
            _configuration = options.Value;
        }

        public async Task<IList<QueueMonitor>> GetQueuesToMonitor()
        {
            var queuesToMonitor = await _memoryCache.RetrieveFromCache<List<QueueMonitor>>(nameof(QueueMonitor));

            if (queuesToMonitor == default(List<QueueMonitor>))
            {
                queuesToMonitor = _configuration
                    .QueueMonitorItems.Split(',')
                    .Select(c=> new QueueMonitor(c, null, _jobEnvironment.EnvironmentName))
                    .ToList();
                await _memoryCache.SaveToCache(nameof(QueueMonitor), queuesToMonitor, 12);
            }

            return queuesToMonitor;
        }

        public async Task<bool> IsQueueHealthy(string queueName)
        {
            var client = new ManagementClient(_configuration.NServiceBusConnectionString);
            var queue = await client.GetQueueRuntimeInfoAsync(queueName);
            
            return queue.MessageCount == 0;
        }

        public async Task SaveQueueStatus(IList<QueueMonitor> queuesToMonitor)
        {
            if (queuesToMonitor != null && queuesToMonitor.Any())
            {
                await _memoryCache.SaveToCache(nameof(QueueMonitor), queuesToMonitor, 12);
            }
        }
    }
}
