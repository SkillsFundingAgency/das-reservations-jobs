using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Notifications
{
    public class NotificationMessage
    {
        public string RecipientsAddress { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
        public string TemplateId { get; set; }
    }
}