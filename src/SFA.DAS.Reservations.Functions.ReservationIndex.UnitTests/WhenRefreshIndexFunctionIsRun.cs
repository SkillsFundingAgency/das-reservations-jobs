using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Reservations;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Reservations.Functions.ReservationIndex.UnitTests
{
    public class WhenRefreshIndexFunctionIsRun
    {
       
        [Test]
        public async Task Then_Message_Handler_Called()
        {
            //Arrange
            var handler = new Mock<IReservationIndexRefreshHandler>();

            //Act
            await IndexRefresh.Run(null, 
                handler.Object,
                Mock.Of<ILogger>());

            //Assert
            handler.Verify(s => s.Handle(), Times.Once);
        }
    }
}