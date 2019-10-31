using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Data.UnitTests.Registry.IndexRegistry
{
    public class WhenIndexesAreDeleted
    {
        private List<IndexRegistryEntry> _expectedRegistryEntries;
        private Mock<ISearchResponse<IndexRegistryEntry>> _response;
        private Mock<IElasticClient> _clientMock;

        [SetUp]
        public void Arrange()
        {
            //Arrange
            _expectedRegistryEntries = new List<IndexRegistryEntry>
            {
                new IndexRegistryEntry{Id = Guid.NewGuid(), Name = "testA", DateCreated = DateTime.Now.AddDays(-5)},
                new IndexRegistryEntry{Id = Guid.NewGuid(), Name = "testB", DateCreated = DateTime.Now.AddDays(-2)},
                new IndexRegistryEntry{Id = Guid.NewGuid(), Name = "testC", DateCreated = DateTime.Now.AddDays(-10)}
            };

            _response = new Mock<ISearchResponse<IndexRegistryEntry>>();
            _response.Setup(x => x.Documents).Returns(_expectedRegistryEntries);

            _clientMock = new Mock<IElasticClient>();
            _clientMock.Setup(x => x.Search(
                    It.IsAny<Func<SearchDescriptor<IndexRegistryEntry>, ISearchRequest>>()))
                .Returns(_response.Object);
        }


        [Test]
        public async Task ThenShouldDeleteAllIndicesOlderThanGivenDays()
        {
            //Arrange
            var oldEntryNames = new List<string> {"testA", "testC"};




            _clientMock.Setup(x => x.BulkAsync(It.IsAny<IBulkRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestBulkResponse(true));

            var registry = new Data.Registry.IndexRegistry(_clientMock.Object, new ReservationJobsEnvironment("LOCAL"));

            //Act
            await registry.DeleteOldIndices(3);

            //Assert
            _clientMock.Verify(x => x.BulkAsync(It.Is<IBulkRequest>(r =>
                r.Operations.OfType<BulkDeleteOperation<IndexRegistryEntry>>()
                    .Select(o => o.Document.Name)
                    .SequenceEqual(oldEntryNames)), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenShouldRemoveCurrentIndexIfAllIndicesDeleted()
        {
            //Arrange
            _clientMock.Setup(x => x.BulkAsync(It.IsAny<IBulkRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestBulkResponse(true));

            var registry = new Data.Registry.IndexRegistry(_clientMock.Object, new ReservationJobsEnvironment("LOCAL"));

            //Act
            await registry.DeleteOldIndices(1);

            //Assert
            Assert.IsNull(registry.CurrentIndexName);
        }

        [Test]
        public async Task ThenShouldNotRemoveCurrentIndexIfAllIndicesDeletionFails()
        {
            //Arrange
            _clientMock.Setup(x => x.BulkAsync(It.IsAny<IBulkRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestBulkResponse(false));

            var registry = new Data.Registry.IndexRegistry(_clientMock.Object, new ReservationJobsEnvironment("LOCAL"));

            //Act
            await registry.DeleteOldIndices(1);

            //Assert
            Assert.AreEqual("testB", registry.CurrentIndexName);
        }

        private class TestBulkResponse : BulkResponse
        {
            public TestBulkResponse(bool isValid)
            {
                IsValid = isValid;
            }
            public override bool IsValid { get; }

        }

    }
}
