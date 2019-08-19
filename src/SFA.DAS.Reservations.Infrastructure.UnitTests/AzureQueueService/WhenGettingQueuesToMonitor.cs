using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Infrastructure.UnitTests.AzureQueueService
{
    public class WhenGettingQueuesToMonitor
    {
        [Test, MoqAutoData]
        public async Task Then_The_Items_Are_Read_from_The_Cache_If_They_Exist(
            [Frozen]Mock<ICacheStorageService> memoryCache,
            List<QueueMonitor> queueMonitorItems,
            AzureServiceBus.AzureQueueService queueService
            )
        {
            //Arrange
            memoryCache.Setup(x => x.RetrieveFromCache<List<QueueMonitor>>(nameof(QueueMonitor)))
                .ReturnsAsync(queueMonitorItems);

            //Act
            var actual = await queueService.GetQueuesToMonitor();

            //Assert
            Assert.AreEqual(queueMonitorItems, actual);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Queues_To_Monitor_Are_Read_From_The_Configuration_If_Not_In_The_Cache_Then_Saved(
            List<string> queueMonitorItems,
            [Frozen]Mock<ICacheStorageService> memoryCache,
            [Frozen]Mock<IOptions<ReservationsJobs>> config,
            AzureServiceBus.AzureQueueService queueService
            )
        {
            //Arrange
            config.Setup(x => x.Value.QueueMonitorItems).Returns(string.Join(",",queueMonitorItems));
            memoryCache.Setup(x => x.RetrieveFromCache<List<QueueMonitor>>(nameof(QueueMonitor))).ReturnsAsync((List<QueueMonitor>)null);

            //Act
            var actual = await queueService.GetQueuesToMonitor();

            //Assert
            //TODO Assert.AreEqual(queueMonitorItems.Count, actual.Count);
            memoryCache.Verify(x=>x.SaveToCache(nameof(QueueMonitor),It.IsAny<List<QueueMonitor>>(),12), Times.Once);
        }
    }
}
