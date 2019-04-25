using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    public class NServiceBusListener : IListener
    {
        private readonly ITriggeredFunctionExecutor _contextExecutor;
        private readonly NServiceBusTriggerAttribute _attribute;

        public NServiceBusListener(ITriggeredFunctionExecutor contextExecutor, NServiceBusTriggerAttribute attribute)
        {
            _contextExecutor = contextExecutor;
            _attribute = attribute;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }
    }
}
