using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    public class NServiceBusTriggerBinding : ITriggerBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly NServiceBusTriggerAttribute _attribute;

        public NServiceBusTriggerBinding(ParameterInfo parameter, NServiceBusTriggerAttribute attribute)
        {
            _parameter = parameter;
            _attribute = attribute;
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            throw new NotImplementedException();
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            throw new NotImplementedException();
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            throw new NotImplementedException();
        }

        public Type TriggerValueType { get; }
        public IReadOnlyDictionary<string, Type> BindingDataContract { get; }
    }
}
