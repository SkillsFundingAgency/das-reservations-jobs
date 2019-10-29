using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Notifications;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface INotificationsService
    {
        Task SendNewReservationMessage(NotificationMessage message);
    }
}