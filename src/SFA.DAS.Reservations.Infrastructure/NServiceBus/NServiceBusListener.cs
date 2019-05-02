using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using NServiceBus;
using NServiceBus.Transport;
using NServiceBus.Raw;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    public class NServiceBusListener : IListener
    {
        private const string PoisonMessageQueue = "error";
        private const int ImmediateRetryCount = 3;

        private readonly ITriggeredFunctionExecutor _executor;
        private readonly NServiceBusTriggerAttribute _attribute;
        private IReceivingRawEndpoint _endpoint;
        private CancellationTokenSource _cancellationTokenSource;

        public NServiceBusListener(ITriggeredFunctionExecutor executor, NServiceBusTriggerAttribute attribute)
        {
            _executor = executor;
            _attribute = attribute;
        }
      
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var endpointConfiguration = RawEndpointConfiguration.Create(_attribute.QueueName, OnMessage, PoisonMessageQueue);
            
            endpointConfiguration.UseTransport<AzureServiceBusTransport>().ConnectionString(_attribute.Connection);
            
            endpointConfiguration.DefaultErrorHandlingPolicy(PoisonMessageQueue, ImmediateRetryCount);

            _endpoint = await RawEndpoint.Start(endpointConfiguration).ConfigureAwait(false);
        }

        protected async Task OnMessage(MessageContext context, IDispatchMessages dispatcher)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var triggerData = new TriggeredFunctionData
            {
                TriggerValue = new NServiceBusTriggerData
                {
                    Data = context.Body,
                    Headers = context.Headers,
                    Dispatcher = dispatcher
                }
            };

            var result = await _executor.TryExecuteAsync(triggerData, _cancellationTokenSource.Token);

            if (!result.Succeeded)
            {
                throw result.Exception;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Cancel();
            return _endpoint.Stop();
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void Dispose()
        {
        }
    }
}
