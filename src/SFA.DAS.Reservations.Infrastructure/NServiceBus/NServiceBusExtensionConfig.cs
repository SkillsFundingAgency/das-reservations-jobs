using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    [Extension("NServiceBus")]
    public class NServiceBusExtensionConfig : IExtensionConfigProvider
    {
        private readonly IOptions<ReservationsJobs> _configuration;

        public NServiceBusExtensionConfig([Inject] IOptions<ReservationsJobs> configuration)
        {
            _configuration = configuration;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<NServiceBusTriggerAttribute>()
                   .BindToTrigger<ConfirmReservationMessage>(new NServiceBusTriggerBindingProvider(_configuration));
        }
    }
}
