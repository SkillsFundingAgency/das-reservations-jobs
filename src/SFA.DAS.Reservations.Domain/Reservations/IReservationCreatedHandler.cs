using System.Threading.Tasks;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationCreatedHandler
    {
        Task Handle(ReservationCreatedEvent createdEvent);
    }
}