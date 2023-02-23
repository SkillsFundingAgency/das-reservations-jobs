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
            //Arrange
            source.Status = ReservationStatus.Confirmed;
            
            //Act
            IndexedReservation result = source;

            //Assert
            result.Should().BeEquivalentTo(source, options => options
                    .Excluding(c=>c.Id)
                    .Excluding(c=>c.CourseName)
                    .Excluding(c=>c.EmployerDeleted)
                    .Excluding(c=>c.EndDate)
                    .Excluding(c => c.Status)
                );
            result.Status.Should().Be((byte)source.Status);
            result.Id.Should().Be($"{0}_{source.AccountLegalEntityId}_{source.Id}");
        }
    }
}