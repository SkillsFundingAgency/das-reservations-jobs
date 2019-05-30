using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.AzureServiceBus
{
    public class AzureQueueService : IAzureQueueService
    {
        private readonly ReservationsJobs _configuration;

        public AzureQueueService(IOptions<ReservationsJobs> options)
        {
            _configuration = options.Value;
        }

        public async Task SendMessage<T>(T message, string queueName) where T : class
        {
            var cloudMessage = new CloudQueueMessage(JsonConvert.SerializeObject(message));

            // Retrieve storage account from connection string.
            var storageAccount = CloudStorageAccount.Parse(_configuration.AzureWebJobsStorage);

            // Create the queue client.
            var queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            var queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn’t already exist
            await queue.CreateIfNotExistsAsync();

            await queue.AddMessageAsync(cloudMessage);
        }
    }
}
