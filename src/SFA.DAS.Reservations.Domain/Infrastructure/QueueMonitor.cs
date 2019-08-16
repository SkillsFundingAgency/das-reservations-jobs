namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public class QueueMonitor
    {
        public string QueueName { get; }

        public bool? IsError { get; set; }
        public string QueueErrorMessage { get; }
        public string QueueNoErrorMessage { get; }

        public QueueMonitor(string queueName, bool? isError)
        {
            QueueName = queueName;
            IsError = isError;
            QueueErrorMessage = $"{QueueName} has entered an error state";
            QueueNoErrorMessage = $"{QueueName} has no errors";
        }

    }
}
