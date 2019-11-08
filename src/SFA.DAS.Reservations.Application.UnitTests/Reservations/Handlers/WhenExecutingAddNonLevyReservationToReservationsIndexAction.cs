using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Handlers
{
    public class WhenExecutingAddNonLevyReservationToReservationsIndexAction
    {
        [Test, MoqAutoData]
        public async Task And_Reservation_Is_Levy_Then_No_Further_Processing(
            Reservation reservation,
            [Frozen] Mock<IReservationService> mockService,
            AddNonLevyReservationToReservationsIndexAction action
        )
        {
            reservation.CourseId = null;
            reservation.CourseLevel = null;
            reservation.CourseName = null;
            reservation.StartDate = DateTime.MinValue;

            await action.Execute(reservation);

            mockService.Verify(service => service.AddReservationToReservationsIndex(reservation),
                Times.Never);
        }

        [Test, MoqAutoData]
        public async Task And_Reservation_Non_Levy_Then_Calls_Service(
            Reservation reservation,
            [Frozen] Mock<IReservationService> mockService,
            AddNonLevyReservationToReservationsIndexAction action
            )
        {
            await action.Execute(reservation);

            mockService.Verify(service => service.AddReservationToReservationsIndex(reservation),
                Times.Once);
        }
    }
}