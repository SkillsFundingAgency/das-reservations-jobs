using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Infrastructure.UnitTests.AzureQueueService
{
    public class WhenSavingQueueStatus
    {
        [Test, MoqAutoData]
        public async Task Then_The_Items_Are_Saved_To_The_Cache(
            [Frozen]Mock<ICacheStorageService> memoryCache,
            IList<QueueMonitor> queueMonitorItems,
            AzureServiceBus.AzureQueueService queueService
            )
        {
            //Act
            await queueService.SaveQueueStatus(queueMonitorItems);
            
            //Assert
            memoryCache.Verify(x=>x.SaveToCache(nameof(QueueMonitor), queueMonitorItems, 12), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Items_Are_Not_Saved_If_It_Is_Empty(
            [Frozen]Mock<ICacheStorageService> memoryCache,
            List<QueueMonitor> queueMonitorItems,
            AzureServiceBus.AzureQueueService queueService
            )
        {
            //Act
            await queueService.SaveQueueStatus(default(List<QueueMonitor>));

            //Assert
            memoryCache.Verify(x => x.SaveToCache(It.IsAny<string>(), It.IsAny<IList<QueueMonitor>>(), It.IsAny<int>()), Times.Never);
        }
    }
}