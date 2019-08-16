using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.AzureServiceBus
{
    public class AzureQueueService : IAzureQueueService
    {
        private readonly ICacheStorageService _memoryCache;
        private readonly ReservationsJobs _configuration;

        public AzureQueueService(IOptions<ReservationsJobs> options, ICacheStorageService memoryCache)
        {
            _memoryCache = memoryCache;
            _configuration = options.Value;
        }

        public async Task<IList<QueueMonitor>> GetQueuesToMonitor()
        {
            var queuesToMonitor = await _memoryCache.RetrieveFromCache<List<QueueMonitor>>(nameof(QueueMonitor));

            if (queuesToMonitor == default(List<QueueMonitor>))
            {
                queuesToMonitor = _configuration.QueueMonitorItems.ToList();
                await _memoryCache.SaveToCache(nameof(QueueMonitor), queuesToMonitor, 12);
            }

            return queuesToMonitor;
        }

        public Task<bool> GetQueueStatus(string expectedQueueName)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveQueueStatus(IList<QueueMonitor> queueMonitors)
        {
            throw new System.NotImplementedException();
        }
    }
}
