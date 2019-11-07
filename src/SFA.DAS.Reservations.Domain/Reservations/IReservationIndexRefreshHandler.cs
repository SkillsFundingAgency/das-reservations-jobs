using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationIndexRefreshHandler
    {
        Task Handle();
    }
}
