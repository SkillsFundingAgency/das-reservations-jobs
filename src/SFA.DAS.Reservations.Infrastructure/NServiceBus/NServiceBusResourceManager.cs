using System.Reflection;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus;

public class NServiceBusResourceManager(IConfiguration _configuration, Logger<NServiceBusResourceManager>? _logger = null)
{
    private readonly ServiceBusAdministrationClient _administrationClient = new ServiceBusAdministrationClient(_configuration["AzureWebJobsServiceBus"]);

    public async Task CreateWorkAndErrorQueues(string queueName)
    {
        await CreateQueue(queueName);
        await CreateQueue($"{queueName}-error");
    }

    private async Task CreateQueue(string queueName)
    {
        if (await _administrationClient.QueueExistsAsync(queueName)) return;

        _logger?.LogInformation("Creating queue: `{queueName}`", queueName);
        await _administrationClient.CreateQueueAsync(queueName);
    }

    public async Task SubscribeToTopicForQueue(Assembly myAssembly, string queueName, string topicName = "bundle-1")
    {
        await CreateSubscription(topicName, queueName);
    }

    private async Task CreateSubscription(string topicName, string queueName)
    {
        if (await _administrationClient.SubscriptionExistsAsync(topicName, queueName)) return;

        _logger?.LogInformation($"Creating subscription to: `{queueName}`", queueName);

        var createSubscriptionOptions = new CreateSubscriptionOptions(topicName, queueName)
        {
            ForwardTo = queueName,
            UserMetadata = $"Subscribed to {queueName}"
        };
        var createRuleOptions = new CreateRuleOptions()
        {
            Filter = new FalseRuleFilter()
        };

        await _administrationClient.CreateSubscriptionAsync(createSubscriptionOptions, createRuleOptions);
    }
}
