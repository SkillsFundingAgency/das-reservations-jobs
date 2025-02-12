using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ReservationIndexRefreshHandler(IReservationService service) : IReservationIndexRefreshHandler
    {
        public async Task Handle()
        {
            await service.RefreshReservationIndex();
        }
    }
}
