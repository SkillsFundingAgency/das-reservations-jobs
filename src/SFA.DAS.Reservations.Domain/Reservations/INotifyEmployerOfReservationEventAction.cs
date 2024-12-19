using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Reservations.Domain.Notifications;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface INotifyEmployerOfReservationEventAction 
    {
        Task Execute<T>(T notificationEvent, IMessageHandlerContext context) where T : INotificationEvent;
    }
}