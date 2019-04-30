using Microsoft.Azure.WebJobs.Host.Config;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    public class NServiceBusExtensionConfig : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<NServiceBusTriggerAttribute>().BindToTrigger<NServiceBusTriggerBindingProvider>();
        }
    }
}
