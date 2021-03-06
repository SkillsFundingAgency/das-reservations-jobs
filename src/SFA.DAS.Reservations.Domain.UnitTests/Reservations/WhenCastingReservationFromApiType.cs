﻿using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Domain.UnitTests.Reservations
{
    public class WhenCastingReservationFromApiType
    {
        [Test, AutoData]
        public void Then_Maps_Matching_Fields(
            int courseLevel,
            ReservationCreatedEvent source)
        {
            source.CourseLevel = courseLevel.ToString();

            Reservation result = source;

            result.Should().BeEquivalentTo(source, options => 
                options.ExcludingMissingMembers()
                    .Excluding(ev => ev.Id)
                    .Excluding(ev => ev.CourseLevel));
            result.Status.Should().Be(ReservationStatus.Pending);
        }
    }
}