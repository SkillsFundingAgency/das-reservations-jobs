using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Domain.Notifications
{
    public interface INotificationTokenBuilder
    {
        Task<Dictionary<string, string>> BuildReservationCreatedTokens(ReservationCreatedEvent createdEvent);
        Task<Dictionary<string, string>> BuildTokens<T>(T notificatinoEvent) where T : INotificationEvent;
    }
}