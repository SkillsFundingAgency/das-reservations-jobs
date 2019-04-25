using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Newtonsoft.Json;
using NServiceBus.Transport;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    public class NServiceBusTriggerBinding : ITriggerBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly NServiceBusTriggerAttribute _attribute;
        private struct BindingNames
        {
            public const string Headers = "headers";
            public const string Dispatcher = "dispatcher";
        }

        public Type TriggerValueType => typeof(NServiceBusTriggerData);

        public IReadOnlyDictionary<string, Type> BindingDataContract => new Dictionary<string, Type>
        {
            {BindingNames.Headers, typeof(Dictionary<string, string>) },
            {BindingNames.Dispatcher, typeof(IDispatchMessages) }
        };

        public NServiceBusTriggerBinding(ParameterInfo parameter, NServiceBusTriggerAttribute attribute)
        {
            _parameter = parameter;
            _attribute = attribute;
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(value is NServiceBusTriggerData triggerData))
            {
                throw new ArgumentException($"Value must be of type {nameof(NServiceBusTriggerData)}", nameof(value));
            }

            object argument;

            try
            {
                var messageText = Encoding.UTF8.GetString(triggerData.Data);
                argument = JsonConvert.DeserializeObject(messageText, _parameter.ParameterType);
            }
            catch (Exception e)
            {
               throw new ArgumentException("Trigger data has invalid payload", nameof(value), e);
            }
            
            var valueBinder = new NServiceBusMessageValueBinder(_parameter, argument);

            var bindingData = new Dictionary<string, object>
            {
                {BindingNames.Headers, triggerData.Headers },
                {BindingNames.Dispatcher, triggerData.Dispatcher }
            };

            return Task.FromResult<ITriggerData>(new TriggerData(valueBinder, bindingData));
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            return Task.FromResult<IListener>(new NServiceBusListener(context.Executor, _attribute));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            //TODO: Change these valies once we know more about the messages we will be processing
            return new ParameterDescriptor
            {
                Name = _parameter.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Prompt = "NsbMessage",
                    Description = "NServiceBus trigger fired",
                    DefaultValue = "Sample"
                }
            };
        }
    }
}
