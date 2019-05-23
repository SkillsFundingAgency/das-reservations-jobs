using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Description;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class NServiceBusTriggerAttribute : Attribute
    {
        public string EndPoint { get; set; }
        public string Connection { get; set; }

        public string EndPointConfigurationTypes { get; set; }
    }
}
