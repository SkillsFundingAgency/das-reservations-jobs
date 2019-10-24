using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Registry;

namespace SFA.DAS.Reservations.Data.UnitTests.Registry.IndexRegistry
{
    public class WhenIndexesAreDeleted
    {
        [Test]
        public async Task ThenShouldDeleteAllIndicesOlderThanGivenDays()
        {
            //Arrange
            var expectedRegistryEntries = new List<IndexRegistryEntry>
            {
                new IndexRegistryEntry{Id = Guid.NewGuid(), Name = "testA", DateCreated = DateTime.Now.AddDays(-5)},
                new IndexRegistryEntry{Id = Guid.NewGuid(), Name = "testB", DateCreated = DateTime.Now.AddDays(-2)},
                new IndexRegistryEntry{Id = Guid.NewGuid(), Name = "testC", DateCreated = DateTime.Now.AddDays(-10)}
            };

            var oldEntryNames = new List<string> {"testA", "testC"};

            var response = new Moq.Mock<Nest.ISearchResponse<IndexRegistryEntry>>();
            response.Setup(x => x.Documents).Returns(expectedRegistryEntries);

            var clientMock = new Mock<IElasticClient>();
            clientMock.Setup(x => x.Search(
                    It.IsAny<Func<SearchDescriptor<IndexRegistryEntry>, ISearchRequest>>()))
                .Returns(response.Object);

            clientMock.Setup(x => x.BulkAsync(It.IsAny<IBulkRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestBulkResponse(true));

            var registry = new Data.Registry.IndexRegistry(clientMock.Object);

            //Act
            await registry.DeleteOldIndices(3);

            //Assert
            clientMock.Verify(x => x.BulkAsync(It.Is<IBulkRequest>(r =>
                r.Operations.OfType<BulkDeleteOperation<IndexRegistryEntry>>()
                    .Select(o => o.Document.Name)
                    .SequenceEqual(oldEntryNames)), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenShouldRemoveCurrentIndexIfAllIndicesDeleted()
        {
            //Arrange
            var expectedRegistryEntries = new List<IndexRegistryEntry>
            {
                new IndexRegistryEntry{Id = Guid.NewGuid(), Name = "testA", DateCreated = DateTime.Now.AddDays(-5)},
                new IndexRegistryEntry{Id = Guid.NewGuid(), Name = "testB", DateCreated = DateTime.Now.AddDays(-2)},
                new IndexRegistryEntry{Id = Guid.NewGuid(), Name = "testC", DateCreated = DateTime.Now.AddDays(-10)}
            };

            var response = new Moq.Mock<Nest.ISearchResponse<IndexRegistryEntry>>();
            response.Setup(x => x.Documents).Returns(expectedRegistryEntries);

            var clientMock = new Mock<IElasticClient>();
            clientMock.Setup(x => x.Search(
                    It.IsAny<Func<SearchDescriptor<IndexRegistryEntry>, ISearchRequest>>()))
                .Returns(response.Object);

            clientMock.Setup(x => x.BulkAsync(It.IsAny<IBulkRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestBulkResponse(true));

            var registry = new Data.Registry.IndexRegistry(clientMock.Object);

            //Act
            await registry.DeleteOldIndices(1);

            //Assert
            Assert.IsNull(registry.CurrentIndexName);
        }

        [Test]
        public async Task ThenShouldNotRemoveCurrentIndexIfAllIndicesDeletionFails()
        {
            //Arrange
            var expectedRegistryEntries = new List<IndexRegistryEntry>
            {
                new IndexRegistryEntry{Id = Guid.NewGuid(), Name = "testA", DateCreated = DateTime.Now.AddDays(-5)},
                new IndexRegistryEntry{Id = Guid.NewGuid(), Name = "testB", DateCreated = DateTime.Now.AddDays(-2)},
                new IndexRegistryEntry{Id = Guid.NewGuid(), Name = "testC", DateCreated = DateTime.Now.AddDays(-10)}
            };

            var response = new Moq.Mock<Nest.ISearchResponse<IndexRegistryEntry>>();
            response.Setup(x => x.Documents).Returns(expectedRegistryEntries);

            var clientMock = new Mock<IElasticClient>();
            clientMock.Setup(x => x.Search(
                    It.IsAny<Func<SearchDescriptor<IndexRegistryEntry>, ISearchRequest>>()))
                .Returns(response.Object);

            clientMock.Setup(x => x.BulkAsync(It.IsAny<IBulkRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestBulkResponse(false));

            var registry = new Data.Registry.IndexRegistry(clientMock.Object);

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
