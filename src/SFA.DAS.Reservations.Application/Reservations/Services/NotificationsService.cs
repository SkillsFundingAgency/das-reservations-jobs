using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.Reservations.Domain.Notifications;

namespace SFA.DAS.Reservations.Application.Reservations.Services
{
    public interface INotificationsService
    {
        Task SendEmail(NotificationMessage message);
    }
    
    public class NotificationsService(IMessageSession publisher) : INotificationsService
    {
        public async Task SendEmail(NotificationMessage message)
        {
            var command = new SendEmailCommand(
                message.TemplateId,
                message.RecipientsAddress,
                message.Tokens
            );

            await publisher.Send(command);
        }
    }
}