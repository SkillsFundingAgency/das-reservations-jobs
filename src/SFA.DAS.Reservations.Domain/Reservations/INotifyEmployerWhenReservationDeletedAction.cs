using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Notifications;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface INotifyEmployerWhenReservationDeletedAction 
    {
        Task Execute<T>(T message) where T : INotificationEvent;
    }
}