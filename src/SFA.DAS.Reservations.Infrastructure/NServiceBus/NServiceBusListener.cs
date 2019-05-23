using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using NServiceBus;
using NServiceBus.Transport;
using NServiceBus.Raw;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.Reservations.Infrastructure.Configuration;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    public class NServiceBusListener : IListener
    {
        private const string PoisonMessageQueue = "error";
        private const int ImmediateRetryCount = 3;

        private readonly ITriggeredFunctionExecutor _executor;
        private readonly NServiceBusTriggerAttribute _attribute;
        private IReceivingRawEndpoint _endpoint;
        private IEndpointInstance _endpointInstance;
        private CancellationTokenSource _cancellationTokenSource;

        public NServiceBusListener(ITriggeredFunctionExecutor executor, NServiceBusTriggerAttribute attribute)
        {
            _executor = executor;
            _attribute = attribute;
        }
      
        public async Task StartAsync(CancellationToken cancellationToken)
        {

            await ConfigureEndpoint();

            var endpointConfigurationRaw = RawEndpointConfiguration.Create(_attribute.EndPoint, OnMessage, PoisonMessageQueue);

            endpointConfigurationRaw.UseTransport<AzureServiceBusTransport>()
                .ConnectionString(_attribute.Connection)
                .Transactions(TransportTransactionMode.ReceiveOnly);

            if (!string.IsNullOrEmpty(EnvironmentVariables.NServiceBusLicense))
            {
                endpointConfigurationRaw.License(EnvironmentVariables.NServiceBusLicense);
            }
            endpointConfigurationRaw.DefaultErrorHandlingPolicy(PoisonMessageQueue, ImmediateRetryCount);

            _endpoint = await RawEndpoint.Start(endpointConfigurationRaw).ConfigureAwait(false);

        }

        private async Task ConfigureEndpoint()
        {
            var endpointConfiguration = new EndpointConfiguration(_attribute.EndPoint)
                .UseAzureServiceBusTransport(_attribute.Connection, r => { })
                .UseInstallers()
                .UseMessageConventions();

            if (!string.IsNullOrEmpty(EnvironmentVariables.NServiceBusLicense))
            {
                endpointConfiguration.License(EnvironmentVariables.NServiceBusLicense);
            }

            _endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            await _endpointInstance.Stop().ConfigureAwait(false);
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


namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class AddedLegalEntityEvent : IEvent
    {
    }

    public class RemovedLegalEntityEvent : IEvent
    {
    }

    public class SignedAgreementEvent : IEvent
    {
    }
    public class HandlerOne : IHandleMessages<AddedLegalEntityEvent>
    {
        public async Task Handle(AddedLegalEntityEvent message, IMessageHandlerContext context)
        {
            return;
        }
    }
    public class HandlerTwo : IHandleMessages<RemovedLegalEntityEvent>
    {
        public async Task Handle(RemovedLegalEntityEvent message, IMessageHandlerContext context)
        {
            return;
        }
    }
    public class HandlerThree : IHandleMessages<SignedAgreementEvent>
    {
        public async Task Handle(SignedAgreementEvent message, IMessageHandlerContext context)
        {
            return;
        }
    }
}
