namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface INotificationService
    {
        void SendNewReservationMessage(ReservationCreatedMessage createdMessage);
    }
}