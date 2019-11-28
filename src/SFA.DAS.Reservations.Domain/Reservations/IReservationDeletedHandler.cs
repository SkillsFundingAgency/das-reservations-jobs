using System.Threading.Tasks;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationDeletedHandler
    {
        Task Handle(ReservationDeletedEvent deletedEvent);
    }
}