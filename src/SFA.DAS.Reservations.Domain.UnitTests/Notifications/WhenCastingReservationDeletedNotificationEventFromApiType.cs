using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Domain.UnitTests.Notifications
{
    public class WhenCastingReservationDeletedNotificationEventFromApiType
    {
        [Test, AutoData]
        public void Then_Maps_Matching_Fields(ReservationDeletedEvent source)
        {
            ReservationDeletedNotificationEvent result = source;

            result.Should().BeEquivalentTo(source, options => 
                options.ExcludingMissingMembers());
        }
    }
}