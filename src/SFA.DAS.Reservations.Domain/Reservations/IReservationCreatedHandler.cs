using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationCreatedHandler
    {
        Task Handle(ReservationCreatedEvent createdEvent, IMessageHandlerContext context);
    }
}