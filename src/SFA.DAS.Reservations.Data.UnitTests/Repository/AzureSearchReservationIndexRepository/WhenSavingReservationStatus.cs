using AutoFixture.NUnit3;
using Azure;
using Azure.Search.Documents.Indexes.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Documents;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.AzureSearchReservationIndexRepository;

public class WhenSavingReservationStatus
{
    [Test, MoqAutoData]
    public async Task Then_Returns_Early_If_Alias_Not_Found(
        Guid reservationId,
        ReservationStatus status,
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        //Arrange
        azureSearchHelper.Setup(x => x.GetAlias(It.IsAny<string>()))
            .ReturnsAsync((SearchAlias)null);

        //Act
        await repository.SaveReservationStatus(reservationId, status);

        //Assert
        azureSearchHelper.Verify(x => x.GetIndex(It.IsAny<string>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_Early_If_Index_Not_Found(
        Guid reservationId,
        ReservationStatus status,
        SearchAlias alias,
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        //Arrange
        azureSearchHelper.Setup(x => x.GetAlias(It.IsAny<string>()))
            .ReturnsAsync(alias);
        azureSearchHelper.Setup(x => x.GetIndex(It.IsAny<string>()))
            .ReturnsAsync((Response<SearchIndex>)null);

        //Act
        await repository.SaveReservationStatus(reservationId, status);

        //Assert
        azureSearchHelper.Verify(x => x.GetDocument(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Logs_Warning_If_Document_Not_Found(
        Guid reservationId,
        ReservationStatus status,
        SearchAlias alias,
        string indexName,
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        [Frozen] Mock<ILogger<Data.Repository.AzureSearchReservationIndexRepository>> mockLogger,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        //Arrange            
        azureSearchHelper.Setup(x => x.GetAlias(It.IsAny<string>()))
            .ReturnsAsync(alias);
        azureSearchHelper.Setup(x => x.GetIndex(It.IsAny<string>()))
          .ReturnsAsync(Response.FromValue<SearchIndex>(value: new SearchIndex(indexName), new Mock<Response>().Object));

        azureSearchHelper.Setup(x => x.GetDocument(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Response<ReservationAzureSearchDocument>)null);

        //Act
        await repository.SaveReservationStatus(reservationId, status);

        //Assert
        mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"Reservation {reservationId} was not found in the index")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()));
    }

    [Test, MoqAutoData]
    public async Task Then_Saves_Document_Status_When_Found(
        Guid reservationId,
        ReservationStatus status,
        SearchAlias alias,
        string indexName,
        ReservationAzureSearchDocument document,
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        [Frozen] Mock<ILogger<Data.Repository.AzureSearchReservationIndexRepository>> mockLogger,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        //Arrange            
        azureSearchHelper.Setup(x => x.GetAlias(It.IsAny<string>()))
            .ReturnsAsync(alias);
        azureSearchHelper.Setup(x => x.GetIndex(It.IsAny<string>()))
            .ReturnsAsync(Response.FromValue<SearchIndex>(value: new SearchIndex(indexName), new Mock<Response>().Object));
        azureSearchHelper.Setup(x => x.GetDocument(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Response.FromValue(document, new Mock<Response>().Object));

        //Act
        await repository.SaveReservationStatus(reservationId, status);

        //Assert            
        azureSearchHelper.Verify(x => x.UploadDocuments(
            It.IsAny<string>(),
            It.Is<IEnumerable<ReservationAzureSearchDocument>>(d =>
               d.First().Status == (int)status)),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Throws_When_Exception_Occurs(
       Guid reservationId,
        SearchAlias alias,
        Exception exception,
        ReservationStatus status,
        [Frozen] Mock<IAzureSearchHelper> azureSearchHelper,
        Data.Repository.AzureSearchReservationIndexRepository repository)
    {
        //Arrange            
        azureSearchHelper.Setup(x => x.GetAlias(It.IsAny<string>()))
            .ReturnsAsync(alias);
        azureSearchHelper.Setup(x => x.GetIndex(It.IsAny<string>()))
            .ReturnsAsync(Response.FromValue<SearchIndex>(value: new SearchIndex("test"), new Mock<Response>().Object));
        azureSearchHelper.Setup(x => x.GetDocument(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(exception);

        //Act Assert 
        Func<Task> act = () => repository.SaveReservationStatus(reservationId, status);
        (await act.Should().ThrowAsync<Exception>()).Which.Should().Be(exception);
    }
}

