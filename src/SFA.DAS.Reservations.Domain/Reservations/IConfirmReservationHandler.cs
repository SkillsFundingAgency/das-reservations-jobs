using System;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IConfirmReservationHandler
    {
        Task Handle(Guid reservationId);
    }
}
