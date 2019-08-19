using System;

namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public class QueueMonitor
    {
        public string QueueName { get; }

        public bool? IsHealthy { get; set; }
        public string QueueErrorMessage { get; }
        public string QueueNoErrorMessage { get; }
        public string Environment { get; }

        public QueueMonitor(string queueName, bool? isHealthy, string environment)
        {
            QueueName = queueName;
            IsHealthy = isHealthy;
            Environment = environment;
            QueueErrorMessage = $"`{QueueName}` in *{environment}* has entered an error state :red_circle: \nat {DateTime.Now:dd MMMM yyyy HH:mm}";
            QueueNoErrorMessage = $"`{QueueName}` in *{environment}* has no errors :heavy_check_mark: \nat {DateTime.Now:dd MMMM yyyy HH:mm}";
        }

    }
}
