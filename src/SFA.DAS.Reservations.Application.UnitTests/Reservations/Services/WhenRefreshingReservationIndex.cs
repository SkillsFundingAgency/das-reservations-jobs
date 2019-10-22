using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;


namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenRefreshingReservationIndex
    {
        private ReservationService _service;
        private Mock<IReservationRepository> _repository;
        private Mock<IReservationIndexRepository> _indexRepository;

        private List<Reservation> _expectedReservations;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IReservationRepository>();
            _indexRepository = new Mock<IReservationIndexRepository>();
            _service = new ReservationService(_repository.Object, _indexRepository.Object);

            _expectedReservations = new List<Reservation>
            {
                new Reservation{ Id = Guid.NewGuid() },
                new Reservation{ Id = Guid.NewGuid() }
            };

            _repository.Setup(x => x.GetAll()).ReturnsAsync(_expectedReservations);
        }

        [Test]
        public async Task ThenAllReservationsWillBeCollected()
        {
            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _repository.Verify(r => r.GetAll(), Times.Once);
        }

        [Test]
        public async Task ThenAllReservationsWillBeIndexed()
        {
            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _indexRepository.Verify(repo => repo.Add(It.Is<IEnumerable<ReservationIndex>>(collection => 
                collection.Any(r => r.Id.Equals(_expectedReservations.First().Id) && 
                                    r.Status.Equals(_expectedReservations.First().Status)) &&
                collection.Any(r => r.Id.Equals(_expectedReservations.Skip(1).First().Id) && 
                                    r.Status.Equals(_expectedReservations.Skip(1).First().Status)) &&
                collection.Count().Equals(2))), Times.Once);
        }

        [Test]
        public async Task ThenIfNoReservationsReturnedIndexingWillBeSkipped()
        {
            //Arrange
            _repository.Setup(x => x.GetAll()).ReturnsAsync(new List<Reservation>());

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _indexRepository.Verify(repo => repo.Add(It.IsAny<IEnumerable<ReservationIndex>>()), Times.Never);
        }

        [Test]
        public void ThenIfExceptionIsThrownFromGettingReservationsIndexingWillBeSkipped()
        {
            //Arrange
            _repository.Setup(x => x.GetAll()).ThrowsAsync(new Exception("Test"));

            //Act
            Assert.ThrowsAsync<Exception>(() => _service.RefreshReservationIndex());

            //Assert
            _indexRepository.Verify(repo => repo.Add(It.IsAny<IEnumerable<ReservationIndex>>()), Times.Never);
        }

        [Test]
        public void ThenIfExceptionIsThrownFromIndexingThenExceptionIsThrown()
        {
            //Arrange
            _indexRepository.Setup(x => x.Add(It.IsAny<IEnumerable<ReservationIndex>>()))
                .ThrowsAsync(new Exception("Test"));

            //Act + Assert
            Assert.ThrowsAsync<Exception>(() => _service.RefreshReservationIndex());
        }
    }
}
