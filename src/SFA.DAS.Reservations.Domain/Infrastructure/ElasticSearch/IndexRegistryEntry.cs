using System;
using Newtonsoft.Json;

namespace SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch
{
    public class IndexRegistryEntry
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime DateCreated { get; set; }
    }
}
