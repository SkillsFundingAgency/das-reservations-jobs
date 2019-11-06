using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IAddNonLevyReservationToReservationsIndexAction
    {
        Task Execute(Reservation reservation);
    }
}