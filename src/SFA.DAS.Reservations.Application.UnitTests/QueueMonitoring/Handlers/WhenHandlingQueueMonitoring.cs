using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.QueueMonitoring.Handlers;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.QueueMonitoring.Handlers
{
    public class WhenHandlingQueueMonitoring
    {
        [Test, MoqAutoData]
        public async Task Then_The_Queue_Details_Are_Retrieved(
            [Frozen]Mock<IAzureQueueService> azureQueueService,
            CheckAvailableQueuesHealth handler)
        {
            //Arrange
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).ReturnsAsync(new List<QueueMonitor>
            {
                new QueueMonitor("test.queue",false, "LOCAL")
            });

            //Act
            await handler.Handle();

            //Assert
            azureQueueService.Verify(x=>x.GetQueuesToMonitor(), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Each_Queue_Is_Checked_And_The_Status_Returned(
            [Frozen]Mock<IAzureQueueService> azureQueueService,
            CheckAvailableQueuesHealth handler)
        {
            //Arrange
            var expectedQueueName = "test.queue";
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).ReturnsAsync(new List<QueueMonitor>
            {
                new QueueMonitor(expectedQueueName,false, "LOCAL")
            });

            //Act
            await handler.Handle();

            //Assert
            azureQueueService.Verify(x=>x.IsQueueHealthy(expectedQueueName), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_Status_Has_Changed_To_Healthy_Then_A_Message_Is_Sent(
            [Frozen]Mock<IAzureQueueService> azureQueueService,
            [Frozen]Mock<IExternalMessagePublisher> externalMessagePublisher,
            CheckAvailableQueuesHealth handler
            )
        {
            //Arrange
            var expectedQueueName = "test.queue";
            var queueMonitor = new QueueMonitor(expectedQueueName,false, "LOCAL");
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).ReturnsAsync(new List<QueueMonitor>
            {
                queueMonitor
            });
            azureQueueService.Setup(x => x.IsQueueHealthy(expectedQueueName)).ReturnsAsync(true);

            //Act
            await handler.Handle();

            //Assert
            azureQueueService.Verify(x => x.IsQueueHealthy(expectedQueueName), Times.Once);
            externalMessagePublisher.Verify(x => x.SendMessage(queueMonitor.QueueNoErrorMessage));
        }


        [Test, MoqAutoData]
        public async Task Then_If_The_Status_Has_Changed_To_Error_Then_A_Message_Is_Sent(
            [Frozen]Mock<IAzureQueueService> azureQueueService,
            [Frozen]Mock<IExternalMessagePublisher> externalMessagePublisher,
            CheckAvailableQueuesHealth handler
        )
        {
            //Arrange
            var expectedQueueName = "test.queue";
            var queueMonitor = new QueueMonitor(expectedQueueName, true, "LOCAL");
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).ReturnsAsync(new List<QueueMonitor>
            {
                queueMonitor
            });
            azureQueueService.Setup(x => x.IsQueueHealthy(expectedQueueName)).ReturnsAsync(false);

            //Act
            await handler.Handle();

            //Assert
            azureQueueService.Verify(x => x.IsQueueHealthy(expectedQueueName), Times.Once);
            externalMessagePublisher.Verify(x => x.SendMessage(queueMonitor.QueueErrorMessage));
        }


        [Test, MoqAutoData]
        public async Task Then_If_The_Status_Has_Not_Changed_A_Message_Is_Not_Sent(
            [Frozen]Mock<IAzureQueueService> azureQueueService,
            [Frozen]Mock<IExternalMessagePublisher> externalMessagePublisher,
            CheckAvailableQueuesHealth handler
        )
        {
            //Arrange
            var expectedQueueName = "test.queue";
            var queueMonitor = new QueueMonitor(expectedQueueName, false, "LOCAL");
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).ReturnsAsync(new List<QueueMonitor>
            {
                queueMonitor
            });
            azureQueueService.Setup(x => x.IsQueueHealthy(expectedQueueName)).ReturnsAsync(false);

            //Act
            await handler.Handle();

            //Assert
            azureQueueService.Verify(x => x.IsQueueHealthy(expectedQueueName), Times.Once);
            externalMessagePublisher.Verify(x => x.SendMessage(It.IsAny<string>()), Times.Never);
        }


        [Test, MoqAutoData]
        public async Task Then_If_The_Status_Has_Not_Been_Set_A_Message_Is_Set(
            [Frozen]Mock<IAzureQueueService> azureQueueService,
            [Frozen]Mock<IExternalMessagePublisher> externalMessagePublisher,
            CheckAvailableQueuesHealth handler
        )
        {
            //Arrange
            var expectedQueueName = "test.queue";
            var queueMonitor = new QueueMonitor(expectedQueueName, null, "LOCAL");
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).ReturnsAsync(new List<QueueMonitor>
            {
                queueMonitor
            });
            azureQueueService.Setup(x => x.IsQueueHealthy(expectedQueueName)).ReturnsAsync(false);

            //Act
            await handler.Handle();

            //Assert
            azureQueueService.Verify(x => x.IsQueueHealthy(expectedQueueName), Times.Once);
            externalMessagePublisher.Verify(x => x.SendMessage(queueMonitor.QueueErrorMessage));
        }

        [Test, MoqAutoData]
        public async Task Then_The_New_Queue_Statuses_Are_Saved(
            [Frozen]Mock<IAzureQueueService> azureQueueService,
            [Frozen]Mock<IExternalMessagePublisher> externalMessagePublisher,
            CheckAvailableQueuesHealth handler
            )
        {
            //Arrange
            var expectedQueueName = "test.queue";
            var queueMonitor = new QueueMonitor(expectedQueueName, true, "LOCAL");
            azureQueueService.Setup(x => x.GetQueuesToMonitor()).ReturnsAsync(new List<QueueMonitor>
            {
                queueMonitor
            });
            azureQueueService.Setup(x => x.IsQueueHealthy(expectedQueueName)).ReturnsAsync(false);

            //Act
            await handler.Handle();

            //Assert
            azureQueueService.Verify(x => x.SaveQueueStatus(It.Is<List<QueueMonitor>>(c=>!c.First().IsHealthy.Value)),Times.Once());
        }
    }
}
