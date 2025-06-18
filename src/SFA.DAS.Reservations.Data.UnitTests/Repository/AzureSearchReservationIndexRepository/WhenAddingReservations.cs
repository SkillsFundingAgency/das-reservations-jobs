using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Documents;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.AzureSearchReservationIndexRepository;

public class WhenAddingReservations
{
    private const string AliasName = "reservations";
    private const string IndexName = "reservations-20240320123456";

    [Test, MoqAutoData]
    public async Task Then_Uploads_Documents_To_Specified_Index(
        List<IndexedReservation> reservations,
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        [Frozen] Mock<ILogger<Data.Repository.AzureSearchReservationIndexRepository>> logger,
        [Frozen] Mock<ReservationsJobs> configuration,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        // Act
        await repository.Add(reservations, IndexName);

        // Assert
        azureSearchHelper.Verify(x => x.UploadDocuments(
            IndexName,
            It.Is<IEnumerable<ReservationAzureSearchDocument>>(docs => 
                docs.Count() == reservations.Count && 
                docs.All(d => reservations.Any(r => r.ReservationId.ToString() == d.ReservationId)))), 
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_Uploads_Documents_To_Alias_Index_When_No_Index_Specified(
        List<IndexedReservation> reservations,
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        [Frozen] Mock<ILogger<Data.Repository.AzureSearchReservationIndexRepository>> logger,
        [Frozen] Mock<ReservationsJobs> configuration,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        // Arrange
        var alias = new SearchAlias(AliasName, new[] { IndexName });
        azureSearchHelper.Setup(x => x.GetAlias(AliasName))
            .ReturnsAsync(alias);

        // Act
        await repository.Add(reservations);

        // Assert
        azureSearchHelper.Verify(x => x.UploadDocuments(
            IndexName,
            It.Is<IEnumerable<ReservationAzureSearchDocument>>(docs => 
                docs.Count() == reservations.Count && 
                docs.All(d => reservations.Any(r => r.ReservationId.ToString() == d.ReservationId)))), 
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_Logs_Warning_And_Skips_Upload_When_Alias_Not_Found(
        List<IndexedReservation> reservations,
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        [Frozen] Mock<ILogger<Data.Repository.AzureSearchReservationIndexRepository>> logger,
        [Frozen] Mock<ReservationsJobs> configuration,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        // Arrange
        azureSearchHelper.Setup(x => x.GetAlias(AliasName))
            .ReturnsAsync((SearchAlias)null);

        // Act
        await repository.Add(reservations);

        // Assert
        logger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Alias 'reservations' not found or has no indexes")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
        
        azureSearchHelper.Verify(x => x.UploadDocuments(
            It.IsAny<string>(),
            It.IsAny<IEnumerable<ReservationAzureSearchDocument>>()),
            Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Logs_Warning_And_Skips_Upload_When_Alias_Has_No_Indexes(
        List<IndexedReservation> reservations,
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        [Frozen] Mock<ILogger<Data.Repository.AzureSearchReservationIndexRepository>> logger,
        [Frozen] Mock<ReservationsJobs> configuration,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        // Arrange
        var alias = new SearchAlias(AliasName, Array.Empty<string>());
        azureSearchHelper.Setup(x => x.GetAlias(AliasName))
            .ReturnsAsync(alias);

        // Act
        await repository.Add(reservations);

        // Assert
        logger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Alias 'reservations' not found or has no indexes")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
        
        azureSearchHelper.Verify(x => x.UploadDocuments(
            It.IsAny<string>(),
            It.IsAny<IEnumerable<ReservationAzureSearchDocument>>()),
            Times.Never);
    }
} 