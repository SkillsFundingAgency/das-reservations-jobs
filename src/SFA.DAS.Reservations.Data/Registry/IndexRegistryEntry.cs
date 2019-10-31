using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Data.Registry
{
    public class IndexRegistryEntry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
