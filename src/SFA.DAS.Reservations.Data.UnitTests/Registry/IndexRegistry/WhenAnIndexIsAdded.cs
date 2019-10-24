using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Registry;

namespace SFA.DAS.Reservations.Data.UnitTests.Registry.IndexRegistry
{
    public class WhenAnIndexIsAdded
    {
        private Data.Registry.IndexRegistry _registry;
        private Mock<IElasticClient> _clientMock;

        [SetUp]
        public void Init()
        {
            _clientMock = new Mock<IElasticClient>();
            _registry = new Data.Registry.IndexRegistry(_clientMock.Object);

            _clientMock.Setup(c => c.IndexAsync(
                It.IsAny<IndexRequest<IndexRegistryEntry>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestIndexResponse(true));
        }

        [Test]
        public async Task ThenShouldAddNewRegistryEntry()
        {
            //Arrange
            var indexName = "TestIndex";

            //Act
            await _registry.Add(indexName);

            //Assert
            _clientMock.Verify(c => c.IndexAsync(It.Is<IndexRequest<IndexRegistryEntry>>(r => 
                r.Document.Name.Equals(indexName)),It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenShouldAddNewRegistryEntryWithCurrentTimestamp()
        {
            //Arrange
            var indexName = "TestIndex";

            //Act
            await _registry.Add(indexName);

            //Assert
            _clientMock.Verify(c => c.IndexAsync(It.Is<IndexRequest<IndexRegistryEntry>>(r => 
                r.Document.DateCreated > DateTime.Now.AddSeconds(-10)),It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenShouldSetCurrentIndexToAddedIndex()
        {
            //Arrange
            await _registry.Add("one");
            await _registry.Add("two");
            await _registry.Add("three");

            //Act
            await _registry.Add("four");

            //Assert
            Assert.AreEqual("four", _registry.CurrentIndexName);
        }

        [Test]
        public async Task ThenShouldNotSetCurrentIndexIfAddingFails()
        {
            //Arrange
            await _registry.Add("one");
            await _registry.Add("two");
            await _registry.Add("three");

            _clientMock.Setup(c => c.IndexAsync(
                    It.IsAny<IndexRequest<IndexRegistryEntry>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestIndexResponse(false));

            //Act
            await _registry.Add("four");

            //Assert
            Assert.AreEqual("three", _registry.CurrentIndexName);
        }

        private class TestIndexResponse : IndexResponse
        {
            public override bool IsValid { get; }

            public TestIndexResponse(bool isValid)
            {
                IsValid = isValid;
            }
        }
    }
}
