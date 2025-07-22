using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Azure.Search.Documents.Indexes.Models;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.AzureSearchReservationIndexRepository;

public class WhenDeletingOldIndices
{
    [Test, MoqAutoData]
    public async Task Then_Skips_When_No_Index_Found(
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        // Arrange
        azureSearchHelper.Setup(x => x.GetIndexes())
            .Returns(Task.FromResult(new List<SearchIndex>()));

        // Act
        await repository.DeleteIndices(5);

        // Assert
        azureSearchHelper.Verify(x => x.DeleteIndex(It.IsAny<string>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Deletes_Indices_Older_Than_Threshold_And_Not_Alias(
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        // Arrange
        var now = DateTime.UtcNow;
        var daysOld = 5u;
        var oldIndex = "reservations-" + now.AddDays(-6).ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var recentIndex = "reservations-" + now.AddDays(-2).ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var indexes = new List<SearchIndex>
                {
                    new SearchIndex(oldIndex),
                    new SearchIndex(recentIndex)
                };
        var alias = new SearchAlias("reservations", recentIndex);

        azureSearchHelper.Setup(x => x.GetIndexes()).Returns(Task.FromResult(indexes));
        azureSearchHelper.Setup(x => x.GetAlias(It.IsAny<string>())).Returns(Task.FromResult(alias));

        // Act
        await repository.DeleteIndices(daysOld);

        // Assert
        azureSearchHelper.Verify(x => x.DeleteIndex(oldIndex), Times.Once);
        azureSearchHelper.Verify(x => x.DeleteIndex(recentIndex), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Does_Not_Delete_Indices_Within_Threshold(
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        // Arrange
        var now = DateTime.UtcNow;
        var daysOld = 5u;
        var recentIndex = "reservations-" + now.AddDays(-2).ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var indexes = new List<SearchIndex>
                {
                    new SearchIndex (recentIndex)
                };
        var alias = new SearchAlias("reservations", "reservations-someother");

        azureSearchHelper.Setup(x => x.GetIndexes()).Returns(Task.FromResult(indexes));
        azureSearchHelper.Setup(x => x.GetAlias(It.IsAny<string>())).Returns(Task.FromResult(alias));

        // Act
        await repository.DeleteIndices(daysOld);

        // Assert
        azureSearchHelper.Verify(x => x.DeleteIndex(recentIndex), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Skips_Deleting_Alias_Target_Even_If_Older(
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        // Arrange
        var now = DateTime.UtcNow;
        var daysOld = 5u;
        var aliasIndex = "reservations-" + now.AddDays(-7).ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var otherOldIndex = "reservations-" + now.AddDays(-8).ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var indexes = new List<SearchIndex>
                {
                    new SearchIndex (aliasIndex),
                    new SearchIndex (otherOldIndex)
                };
        var alias = new SearchAlias("reservations", aliasIndex);

        azureSearchHelper.Setup(x => x.GetIndexes()).Returns(Task.FromResult(indexes));
        azureSearchHelper.Setup(x => x.GetAlias(It.IsAny<string>())).Returns(Task.FromResult(alias));

        // Act
        await repository.DeleteIndices(daysOld);

        // Assert
        azureSearchHelper.Verify(x => x.DeleteIndex(aliasIndex), Times.Never);
        azureSearchHelper.Verify(x => x.DeleteIndex(otherOldIndex), Times.Once);
    }
}

