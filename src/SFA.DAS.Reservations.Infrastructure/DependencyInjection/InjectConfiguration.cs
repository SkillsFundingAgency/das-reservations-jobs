using Microsoft.Azure.WebJobs.Host.Config;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Infrastructure.DependencyInjection
{
    internal class InjectConfiguration : IExtensionConfigProvider
    {
        public readonly InjectBindingProvider InjectBindingProvider;

        public InjectConfiguration(InjectBindingProvider injectBindingProvider) =>
            InjectBindingProvider = injectBindingProvider;

        public void Initialize(ExtensionConfigContext context) => context
            .AddBindingRule<InjectAttribute>()
            .Bind(InjectBindingProvider);
    }
}