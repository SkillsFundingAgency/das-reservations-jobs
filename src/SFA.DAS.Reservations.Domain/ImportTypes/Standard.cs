using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.ImportTypes
{
    public class StandardApiResponse
    {
        public List<Standard> Standards { get; set; }
    }
    
    public class Standard
    {
        public DateTime EffectiveTo { get ; set ; }

        public string Title { get ; set ; }

        public int Level { get ; set ; }

        public int Id { get ; set ; }
    }
}