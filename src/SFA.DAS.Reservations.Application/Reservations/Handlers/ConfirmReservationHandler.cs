using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ConfirmReservationHandler : IConfirmReservationHandler
    {
        public Task Handle(Guid reservationId)
        {
            throw new NotImplementedException();
        }
    }
}
