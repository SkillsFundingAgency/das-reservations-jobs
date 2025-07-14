using AutoFixture.NUnit3;
using Azure;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Documents;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.AzureSearchReservationIndexRepository
{
    public class WhenDeletingReservationsFromIndex
    {

        [Test, MoqAutoData]
        public async Task Then_Returns_Early_If_Alias_Is_Null(
            uint ukPrn,
            long accountLegalEntityId,
            [Frozen] Mock<IAzureSearchHelper> azureSearchHelperMock,
            Data.Repository.AzureSearchReservationIndexRepository repository
            )
        {
            // Arrange
            azureSearchHelperMock.Setup(x => x.GetAlias(It.IsAny<string>()))
                .ReturnsAsync((SearchAlias)null);

            // Act  
            await repository.DeleteReservationsFromIndex(ukPrn, accountLegalEntityId);

            // Assert 
            azureSearchHelperMock.Verify(x => x.GetAlias(It.IsAny<string>()), Times.Once);
            azureSearchHelperMock.Verify(x => x.GetIndex(It.IsAny<string>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Early_If_Index_Is_Null(
            uint ukPrn,
            long accountLegalEntityId,
            [Frozen] Mock<IAzureSearchHelper> azureSearchHelperMock,
            Data.Repository.AzureSearchReservationIndexRepository repository)
        {
            // Arrange                                 
            azureSearchHelperMock.Setup(x => x.GetIndex(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            await repository.DeleteReservationsFromIndex(ukPrn, accountLegalEntityId);

            // Assert            
            azureSearchHelperMock.Verify(x => x.SearchDocuments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
            azureSearchHelperMock.Verify(x => x.DeleteDocuments(It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Early_If_Index_Value_Is_Null(
            uint ukPrn,
            long accountLegalEntityId,
            [Frozen] Mock<IAzureSearchHelper> azureSearchHelperMock,
            Data.Repository.AzureSearchReservationIndexRepository repository)
        {
            // Arrange                       
            azureSearchHelperMock.Setup(x => x.GetIndex(It.IsAny<string>()))
                .ReturnsAsync(Response.FromValue<SearchIndex>(value: null, new Mock<Response>().Object));

            // Act
            await repository.DeleteReservationsFromIndex(ukPrn, accountLegalEntityId);

            // Assert            
            azureSearchHelperMock.Verify(x => x.SearchDocuments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
            azureSearchHelperMock.Verify(x => x.DeleteDocuments(It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Deletes_Documents_When_Documents_Exist(
            uint ukPrn,
            long accountLegalEntityId,
            string documentId,
            [Frozen] Mock<IAzureSearchHelper> azureSearchHelperMock,
            Data.Repository.AzureSearchReservationIndexRepository repository,
            ReservationAzureSearchDocument document)
        {
            // Arrange            
            azureSearchHelperMock.Setup(x => x.GetIndex(It.IsAny<string>()))
             .ReturnsAsync(Response.FromValue<SearchIndex>(value: new SearchIndex("test"), new Mock<Response>().Object));

            var searchResult = SearchModelFactory.SearchResult(
                       document: document,
                       score: 1.0,
                       highlights: null,
                       semanticSearch: null,
                       documentDebugInfo: null);

            azureSearchHelperMock.Setup(
                x => x.SearchDocuments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
                .ReturnsAsync(
                    SearchModelFactory.SearchResults<ReservationAzureSearchDocument>(
                        values: new[] { searchResult },
                        totalCount: 1,
                        facets: null,
                        coverage: 1.0,
                        rawResponse: null));

            // Act
            await repository.DeleteReservationsFromIndex(ukPrn, accountLegalEntityId);

            // Assert            
            azureSearchHelperMock.Verify(x => x.SearchDocuments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
            azureSearchHelperMock.Verify(x => x.DeleteDocuments(It.IsAny<string>(), It.Is<IEnumerable<string>>(ids => ids.Contains(document.Id))), Times.Once);
        }

        [Test, MoqAutoData]
        public void Throws_When_Exception_Occurs(
            uint ukPrn,
            long accountLegalEntityId,
            Exception exception,
            [Frozen] Mock<IAzureSearchHelper> azureSearchHelperMock,
            Data.Repository.AzureSearchReservationIndexRepository repository)
        {
            // Arrange
            azureSearchHelperMock.Setup(x => x.GetIndex(It.IsAny<string>()))
            .ReturnsAsync(Response.FromValue<SearchIndex>(value: new SearchIndex("test"), new Mock<Response>().Object));

            azureSearchHelperMock.Setup(x => x.SearchDocuments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
                .ThrowsAsync(exception);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => repository.DeleteReservationsFromIndex(ukPrn, accountLegalEntityId));
            Assert.That(ex, Is.EqualTo(exception));
        }
    }
}
