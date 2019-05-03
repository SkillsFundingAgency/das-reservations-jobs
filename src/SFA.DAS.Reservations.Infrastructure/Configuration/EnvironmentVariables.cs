using System;

namespace SFA.DAS.Reservations.Infrastructure.Configuration
{
    public class EnvironmentVariables
    {
        public static string NServiceBusConnectionString = Environment.GetEnvironmentVariable("NServiceBusConnectionString");
    }
}
