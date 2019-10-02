using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class ReservationCreatedMessage
    {
        public string RecipientsAddress { get; set; }
        public Dictionary<string, string> Tokens { get; set; }//provider_name, date description, course description, deeplink_manage (req hashed account id)
        public string TemplateId { get; set; } //todo: get from config
        public string SystemId { get; set; }//das-reservations-jobs? //todo: needed? - from config
    }
}