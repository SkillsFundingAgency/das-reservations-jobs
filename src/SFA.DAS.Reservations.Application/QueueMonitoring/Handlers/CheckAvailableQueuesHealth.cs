using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Application.QueueMonitoring.Handlers
{
    public class CheckAvailableQueuesHealth : ICheckAvailableQueuesHealth
    {
        private readonly IAzureQueueService _azureQueueService;
        private readonly IExternalMessagePublisher _externalMessagePublisher;

        public CheckAvailableQueuesHealth(IAzureQueueService azureQueueService, IExternalMessagePublisher externalMessagePublisher)
        {
            _azureQueueService = azureQueueService;
            _externalMessagePublisher = externalMessagePublisher;
        }
        public async Task Handle()
        {
            var queues = await _azureQueueService.GetQueuesToMonitor();

            foreach (var queue in queues)
            {
                var queueStatus = await _azureQueueService.IsQueueHealthy(queue.QueueName);

                if (queue.IsHealthy.HasValue && queueStatus == queue.IsHealthy)
                {
                    continue;
                }

                queues[queues.IndexOf(queue)].IsHealthy = queueStatus;

                if (queueStatus)
                {
                    await _externalMessagePublisher.SendMessage(queue.QueueNoErrorMessage);
                }
                else
                {
                    await _externalMessagePublisher.SendMessage(queue.QueueErrorMessage);
                }
            }

            await _azureQueueService.SaveQueueStatus(queues);
        }
    }
}
