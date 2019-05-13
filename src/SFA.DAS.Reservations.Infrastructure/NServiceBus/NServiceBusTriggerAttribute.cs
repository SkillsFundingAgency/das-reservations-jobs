using System;
using Microsoft.Azure.WebJobs.Description;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class NServiceBusTriggerAttribute : Attribute
    {
        public string QueueName { get; set; }
        public string Connection { get; set; }
    }
}
