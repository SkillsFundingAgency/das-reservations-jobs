using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Domain.UnitTests.Infrastructure
{
    public class WhenBuildingAQueueMonitorItem
    {
        [Test]
        public void Then_The_Messages_Are_Set_On_The_Model()
        {
            //Act
            var actual = new QueueMonitor("queue.name", true, "LOCAL");

            //Assert
            Assert.IsTrue(actual.QueueErrorMessage.StartsWith("`queue.name` in *LOCAL* has entered an error state :red_circle: at "));
            Assert.IsTrue(actual.QueueNoErrorMessage.StartsWith("`queue.name` in *LOCAL* has no errors :heavy_check_mark: at "));
        }
    }
}
