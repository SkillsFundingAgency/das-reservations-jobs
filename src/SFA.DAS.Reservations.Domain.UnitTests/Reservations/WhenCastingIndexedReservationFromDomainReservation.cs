using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Domain.UnitTests.Reservations
{
    public class WhenCastingIndexedReservationFromDomainReservation
    {
        [Test, AutoData]
        public void Then_Maps_Matching_Fields(
            Reservation source)
        {
            IndexedReservation result = source;

            result.Should().BeEquivalentTo(source, options => 
                options.ExcludingMissingMembers()
                    .Excluding(ev => ev.Id));
        }
    }
}