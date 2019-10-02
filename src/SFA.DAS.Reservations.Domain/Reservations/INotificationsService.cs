namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface INotificationsService
    {
        void SendNewReservationMessage(ReservationCreatedMessage createdMessage);
    }
}