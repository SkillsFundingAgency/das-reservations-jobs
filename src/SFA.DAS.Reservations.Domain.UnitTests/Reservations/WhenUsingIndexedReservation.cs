using System;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Domain.UnitTests.Reservations
{
    public class WhenUsingIndexedReservation
    {
        [Test]
        public void Then_Creates_Reservation_Period_With_Start_And_Expiry_Dates()
        {
            //Arrange
            var reservation = new IndexedReservation
            {
                StartDate = new DateTime(2019, 09,01),
                ExpiryDate = new DateTime(2019,12,01)
            };

            //Act + Assert
            Assert.AreEqual("Sep 2019 to Dec 2019",  reservation.ReservationPeriod);
        }

        [Test]
        public void Then_Creates_Reservation_Period_With_Start_Date()
        {
            //Arrange
            var reservation = new IndexedReservation
            {
                StartDate = new DateTime(2019, 09,01)
                
            };

            //Act + Assert
            Assert.AreEqual("Sep 2019",  reservation.ReservationPeriod);
        }
        
        [Test]
        public void Then_Has_Empty_Period_If_Not_Start_Or_Expiry_Date()
        {
            //Arrange
            var reservation = new IndexedReservation();

            //Act + Assert
            Assert.AreEqual(string.Empty,  reservation.ReservationPeriod);
        }
    }
}
