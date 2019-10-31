using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Handlers
{
    public class WhenRefreshingReservationIndex
    {
        private ReservationIndexRefreshHandler _handler;
        private Mock<IReservationService> _service;

        [SetUp]
        public void Arrange()
        {
            _service = new Mock<IReservationService>();
            _handler = new ReservationIndexRefreshHandler(_service.Object);
        }

        [Test]
        public async Task ThenSetsReservationStatusToConfirmed()
        {
            //Act
            await _handler.Handle();

            //Assert
            _service.Verify(s => s.RefreshReservationIndex(), Times.Once);
        }
    }
}
