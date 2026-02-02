using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.AzureServiceBus;

public class AzureQueueService(IOptions<ReservationsJobs> options) : IAzureQueueService
{
    private readonly ReservationsJobs _configuration = options.Value;

    public async Task SendMessage<T>(T message, string queueName) where T : class
    {
        var queueClient = new QueueClient(_configuration.AzureWebJobsStorage, queueName);
        await queueClient.CreateIfNotExistsAsync();
        await queueClient.SendMessageAsync(JsonConvert.SerializeObject(message));
    }
}