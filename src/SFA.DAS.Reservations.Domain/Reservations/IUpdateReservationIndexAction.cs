using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IUpdateReservationIndexAction
    {
        Task Execute(Reservation reservation);
    }
}