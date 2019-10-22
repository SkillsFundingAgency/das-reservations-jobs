using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ReservationIndexRefreshHandler : IReservationIndexRefreshHandler
    {
        private readonly IReservationService _service;

        public ReservationIndexRefreshHandler(IReservationService service)
        {
            _service = service;
        }

        public async Task Handle()
        {
            await _service.RefreshReservationIndex();
        }
    }
}
