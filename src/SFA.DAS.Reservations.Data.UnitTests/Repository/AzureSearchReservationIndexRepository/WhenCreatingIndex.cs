using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.AzureSearchReservationIndexRepository
{
    public class WhenCreatingIndex
    {
        [Test, MoqAutoData]
        public async Task Then_The_Index_Is_Created_And_Returned(
            [Frozen] Mock<IAzureSearchHelper> azureSearchHelperMock,
            Data.Repository.AzureSearchReservationIndexRepository repository)
        {
            // Arrange
            azureSearchHelperMock
                .Setup(x => x.CreateIndex(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await repository.CreateIndex();

            // Assert
            StringAssert.StartsWith("reservations-", result);
            azureSearchHelperMock.Verify(x => x.CreateIndex(result), Times.Once);
        }
    }
}
