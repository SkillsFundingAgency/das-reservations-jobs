using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.AzureSearchReservationIndexRepository;
public class WhenUpdatingAlias
{
    private const string AliasName = "reservations";
    private const string IndexName = "reservations-123";

    [Test, MoqAutoData]
    public async Task Then_Alias_Is_Created_Or_Updated(
    [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
    Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        // Act
        await repository.UpdateAlias(IndexName);

        // Assert
        azureSearchHelper.Verify(x => x.UpdateAlias(
            AliasName,
            IndexName),
            Times.Once);
    }
}
