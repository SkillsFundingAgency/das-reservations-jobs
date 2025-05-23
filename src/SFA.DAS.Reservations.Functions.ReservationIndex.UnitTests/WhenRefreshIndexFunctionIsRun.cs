using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Reservations;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Functions.ReservationIndex.Functions;

namespace SFA.DAS.Reservations.Functions.ReservationIndex.UnitTests
{
    public class WhenRefreshIndexFunctionIsRun
    {
       
        [Test]
        public async Task Then_Message_Handler_Called()
        {
            //Arrange
            var handler = new Mock<IReservationIndexRefreshHandler>();

            var refreshIndex = new RefreshIndex(Mock.Of<ILogger<ReservationIndexRefreshHandler>>(), handler.Object);

            //Act
            await refreshIndex.Run(null);

            //Assert
            handler.Verify(s => s.Handle(), Times.Once);
        }
    }
}