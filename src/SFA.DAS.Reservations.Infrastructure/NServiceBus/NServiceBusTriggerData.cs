 using System.Collections.Generic;
using NServiceBus.Transport;

namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    public class NServiceBusTriggerData
    {
        public byte[] Data { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public IDispatchMessages Dispatcher { get; set; }
    }
}
