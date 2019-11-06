using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Handlers
{
    public class WhenExecutingUpdateReservationIndexAction
    {
        [Test, MoqAutoData]
        public async Task Then_Calls_Service(
            Reservation reservation,
            [Frozen] Mock<IReservationService> mockService,
            UpdateReservationIndexAction action
            )
        {
            await action.Execute(reservation);

            mockService.Verify(service => service.AddReservationToReservationsIndex(reservation));
        }
    }
}