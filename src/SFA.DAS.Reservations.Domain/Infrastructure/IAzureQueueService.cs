using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public interface IAzureQueueService
    {
        Task SendMessage<T>(T message, string queueName) where T : class;
    }
}
