using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Notifications
{
    public interface INotificationTokenBuilder
    {
        Task<Dictionary<string, string>> BuildTokens<T>(T notificationEvent) where T : INotificationEvent;
    }
}