//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using Moq;
//using NUnit.Framework;
//using SFA.DAS.EmployerAccounts.Messages.Events;
//using SFA.DAS.Reservations.Domain.Accounts;

//namespace SFA.DAS.Reservations.Functions.LegalEntities.UnitTests
//{
//    public class WhenUpdatedAccountNameEventTriggered
//    {
//        [Test]
//        public async Task Then_The_Message_Will_Be_Handled()
//        {
//            //Arrange
//            var handler = new Mock<IAccountNameUpdatedHandler>();
//            var message = new ChangedAccountNameEvent{AccountId = 1 , CurrentName = "Test"};
            
//            //Act
//            await HandleAccountNameUpdatedEvent.Run(message, handler.Object,Mock.Of<ILogger<ChangedAccountNameEvent>>());

//            //Assert
//            handler.Verify(x=>x.Handle(
//                It.Is<ChangedAccountNameEvent>(c=>c.AccountId.Equals(message.AccountId) 
//                                                  && c.CurrentName.Equals(message.CurrentName))));
//        }
//    }
//}