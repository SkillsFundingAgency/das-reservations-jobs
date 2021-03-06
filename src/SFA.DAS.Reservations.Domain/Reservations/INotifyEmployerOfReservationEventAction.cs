﻿using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Notifications;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface INotifyEmployerOfReservationEventAction 
    {
        Task Execute<T>(T notificationEvent) where T : INotificationEvent;
    }
}