using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    [Extension("NServiceBus")]
    public class NServiceBusExtensionConfig : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<NServiceBusTriggerAttribute>()
                   .BindToTrigger<ConfirmReservationMessage>(new NServiceBusTriggerBindingProvider());
        }
    }
}
