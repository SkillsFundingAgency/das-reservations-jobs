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
            var actual = new QueueMonitor("queue.name", true);

            //Assert
            Assert.AreEqual("queue.name has entered an error state", actual.QueueErrorMessage);
            Assert.AreEqual("queue.name has no errors", actual.QueueNoErrorMessage);
        }
    }
}
