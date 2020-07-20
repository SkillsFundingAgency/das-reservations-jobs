using System;

namespace SFA.DAS.Reservations.Domain.ImportTypes
{
    public class Standard
    {
        public DateTime EffectiveTo { get ; set ; }

        public string Title { get ; set ; }

        public int Level { get ; set ; }

        public int Id { get ; set ; }
    }
}