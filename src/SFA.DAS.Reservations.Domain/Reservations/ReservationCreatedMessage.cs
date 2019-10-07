using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class ReservationCreatedMessage
    {
        public string RecipientsAddress { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
        public string TemplateId { get; set; }
    }
}