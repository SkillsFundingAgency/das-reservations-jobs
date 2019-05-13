using System;
using NServiceBus.Raw;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    public static class RawEndpointConfigurationExtension
    {
        public static void License(this RawEndpointConfiguration config, string licenseText)
        {
            if (string.IsNullOrEmpty(licenseText))
            {
                throw new ArgumentException("NServiceBus license text much not be empty", nameof(licenseText));
            }

            config.Settings.Set("LicenseText", (object) licenseText);
        }
    }
}
