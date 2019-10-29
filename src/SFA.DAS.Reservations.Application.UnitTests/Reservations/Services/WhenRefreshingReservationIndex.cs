using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenRefreshingReservationIndex
    {
        private ReservationService _service;
        private Mock<IReservationRepository> _repository;
        private Mock<IReservationIndexRepository> _indexRepository;
        private Mock<IProviderPermissionsRepository> _permissionsRepository;

        private List<Reservation> _expectedReservations;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IReservationRepository>();
            _indexRepository = new Mock<IReservationIndexRepository>();
            _permissionsRepository = new Mock<IProviderPermissionsRepository>();

            _service = new ReservationService(_repository.Object, _indexRepository.Object, _permissionsRepository.Object);

            _expectedReservations = new List<Reservation>
            {
                new Reservation{ Id = Guid.NewGuid() },
                new Reservation{ Id = Guid.NewGuid() }
            };

            _repository.Setup(x => x.GetAll()).Returns(_expectedReservations);
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
        public async Task ThenANewIndexWillBeCreated()
        {
            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _indexRepository.Verify(r => r.CreateIndex(), Times.Once);
        }

        [Test]
        public async Task ThenIfNoReservationsReturnedIndexingWillBeSkipped()
        {
            //Arrange
            _repository.Setup(x => x.GetAll()).Returns(new List<Reservation>());

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _indexRepository.Verify(repo => repo.Add(It.IsAny<IEnumerable<ReservationIndex>>()), Times.Never);
        }

        [Test]
        public void ThenIfExceptionIsThrownFromGettingReservationsIndexingWillBeSkipped()
        {
            //Arrange
            _repository.Setup(x => x.GetAll()).Throws(new Exception("Test"));

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

        [Test]
        public async Task ThenAllLinkedProvidersWillHaveReservationCopiesCreated()
        {
            //Arrange
            var firstReservationId = Guid.NewGuid();
            var secondReservationId = Guid.NewGuid();

            _expectedReservations = new List<Reservation>
            {
                new Reservation {Id = firstReservationId, AccountId = 1, ProviderId = 1, AccountLegalEntityId = 1},
                new Reservation {Id = secondReservationId, AccountId = 1, ProviderId = 2, AccountLegalEntityId = 1}
            };
            _repository.Setup(x => x.GetAll()).Returns(_expectedReservations);

            _permissionsRepository.Setup(r => r.GetAll()).Returns(new List<ProviderPermission>
            {
                new ProviderPermission {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 1, CanCreateCohort = true},
                new ProviderPermission {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 2, CanCreateCohort = true}
            });

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _indexRepository.Verify(
                x => x.Add(It.Is<IEnumerable<ReservationIndex>>(rIndex => rIndex.Count().Equals(4))));

            _indexRepository.Verify(x => x.Add(It.Is<IEnumerable<ReservationIndex>>(rIndex =>
                rIndex.Any(r =>
                    r.Id.Equals(firstReservationId) &&
                    r.AccountId.Equals(1) &&
                    r.ProviderId.Value.Equals(1) &&
                    r.AccountLegalEntityId.Equals(1)))));

            _indexRepository.Verify(x => x.Add(It.Is<IEnumerable<ReservationIndex>>(rIndex =>
                rIndex.Any(r =>
                    r.Id.Equals(firstReservationId) &&
                    r.AccountId.Equals(1) &&
                    r.ProviderId.Value.Equals(1) &&
                    r.AccountLegalEntityId.Equals(1)))));

            _indexRepository.Verify(x => x.Add(It.Is<IEnumerable<ReservationIndex>>(rIndex =>
                rIndex.Any(r =>
                    r.Id.Equals(secondReservationId) &&
                    r.AccountId.Equals(1) &&
                    r.ProviderId.Value.Equals(1) &&
                    r.AccountLegalEntityId.Equals(1)))));

            _indexRepository.Verify(x => x.Add(It.Is<IEnumerable<ReservationIndex>>(rIndex =>
                rIndex.Any(r =>
                        r.Id.Equals(secondReservationId) &&
                        r.AccountId.Equals(1) &&
                        r.ProviderId.Value.Equals(2) &&
                        r.AccountLegalEntityId.Equals(1)))));
        }

        [Test]
        public async Task ThenAnyProvidersWithPermissionsWillHaveCopiesOfExistingReservationsAdded()
        {
            //Arrange
            var firstReservationId = Guid.NewGuid();

            _expectedReservations = new List<Reservation>
            {
                new Reservation {Id = firstReservationId, AccountId = 1, ProviderId = 1, AccountLegalEntityId = 1},
            };
            _repository.Setup(x => x.GetAll()).Returns(_expectedReservations);

            _permissionsRepository.Setup(r => r.GetAll()).Returns(new List<ProviderPermission>
            {
                new ProviderPermission {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 1, CanCreateCohort = true},
                new ProviderPermission {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 2, CanCreateCohort = true}
            });

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _indexRepository.Verify(
                x => x.Add(It.Is<IEnumerable<ReservationIndex>>(rIndex => rIndex.Count().Equals(2))));

            _indexRepository.Verify(x => x.Add(It.Is<IEnumerable<ReservationIndex>>(rIndex =>
                rIndex.Any(r =>
                    r.Id.Equals(firstReservationId) &&
                    r.AccountId.Equals(1) &&
                    r.ProviderId.Value.Equals(1) &&
                    r.AccountLegalEntityId.Equals(1)))));

            _indexRepository.Verify(x => x.Add(It.Is<IEnumerable<ReservationIndex>>(rIndex =>
                rIndex.Any(r =>
                    r.Id.Equals(firstReservationId) &&
                    r.AccountId.Equals(1) &&
                    r.ProviderId.Value.Equals(1) &&
                    r.AccountLegalEntityId.Equals(1)))));
        }

        [Test]
        public async Task ThenNoCopiesAreCreatedIfNoReservationsExist()
        {
            //Arrange
            _expectedReservations = new List<Reservation>();
            _repository.Setup(x => x.GetAll()).Returns(_expectedReservations);

            _permissionsRepository.Setup(r => r.GetAll()).Returns(new List<ProviderPermission>
            {
                new ProviderPermission {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 1, CanCreateCohort = true},
                new ProviderPermission {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 2, CanCreateCohort = true}
            });

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _indexRepository.Verify(x => x.Add(It.IsAny<IEnumerable<ReservationIndex>>()), Times.Never());
        }

        [Test]
        public async Task ThenNoCopiesAreCreatedForProvidersWhoHavePermissionsWithAnotherLegalEntity()
        {
            //Arrange
            var firstReservationId = Guid.NewGuid();
            
            _expectedReservations = new List<Reservation>
            {
                new Reservation {Id = firstReservationId, AccountId = 1, ProviderId = 1, AccountLegalEntityId = 1}
            };
            _repository.Setup(x => x.GetAll()).Returns(_expectedReservations);

            _permissionsRepository.Setup(r => r.GetAll()).Returns(new List<ProviderPermission>
            {
                new ProviderPermission {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 1, CanCreateCohort = true},
                new ProviderPermission {AccountId = 1, AccountLegalEntityId = 2, ProviderId = 2, CanCreateCohort = true}
            });

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _indexRepository.Verify(
                x => x.Add(It.Is<IEnumerable<ReservationIndex>>(rIndex => rIndex.Count().Equals(1))));

            _indexRepository.Verify(x => x.Add(It.Is<IEnumerable<ReservationIndex>>(rIndex =>
                rIndex.Any(r =>
                    r.Id.Equals(firstReservationId) &&
                    r.AccountId.Equals(1) &&
                    r.ProviderId.Value.Equals(1) &&
                    r.AccountLegalEntityId.Equals(1)))));
        }

        [Test]
        public async Task ThenNoProvidersPermissionsAreFoundNoCopiesAreCreated()
        {
            //Arrange
            var firstReservationId = Guid.NewGuid();
            
            _expectedReservations = new List<Reservation>
            {
                new Reservation {Id = firstReservationId, AccountId = 1, ProviderId = 1, AccountLegalEntityId = 1}
            };
            _repository.Setup(x => x.GetAll()).Returns(_expectedReservations);

            _permissionsRepository.Setup(r => r.GetAll()).Returns(new List<ProviderPermission>());

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _indexRepository.Verify(
                x => x.Add(It.Is<IEnumerable<ReservationIndex>>(rIndex => rIndex.Count().Equals(1))));

            _indexRepository.Verify(x => x.Add(It.Is<IEnumerable<ReservationIndex>>(rIndex =>
                rIndex.Any(r =>
                    r.Id.Equals(firstReservationId) &&
                    r.AccountId.Equals(1) &&
                    r.ProviderId.Value.Equals(1) &&
                    r.AccountLegalEntityId.Equals(1)))));
        }

    }
}
