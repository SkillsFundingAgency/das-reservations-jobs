using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public class ConfigurationItem : TableEntity
    {
        public string Data { get; set; }
    }
}