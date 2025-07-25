﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenRefreshingReservationIndex
    {
        private ReservationService _service;
        private Mock<IReservationRepository> _repository;
        private Mock<IElasticReservationIndexRepository> _elasticIndexRepository;
        private Mock<IAzureSearchReservationIndexRepository> _azureSearchIndexRepository;
        private Mock<IProviderPermissionRepository> _permissionsRepository;

        private List<Reservation> _expectedReservations;
        private Mock<ILogger<ReservationService>> _logger;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IReservationRepository>();
            _elasticIndexRepository = new Mock<IElasticReservationIndexRepository>();
            _azureSearchIndexRepository = new Mock<IAzureSearchReservationIndexRepository>();
            _permissionsRepository = new Mock<IProviderPermissionRepository>();
            _logger = new Mock<ILogger<ReservationService>>();

            _expectedReservations =
            [
                new() { Id = Guid.NewGuid(), AccountLegalEntityId = 1 },
                new() { Id = Guid.NewGuid(), AccountLegalEntityId = 1 }
            ];

            _repository.Setup(x => x.GetAllNonLevyForAccountLegalEntity(2)).Returns(_expectedReservations);

            _permissionsRepository.Setup(r => r.GetAllWithCreateCohortPermission()).Returns(new List<Domain.Entities.ProviderPermission>
            {
                new() {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 1, CanCreateCohort = true},
                new() {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 2, CanCreateCohort = true}
            });

            _service = new ReservationService(
                _repository.Object,
                _elasticIndexRepository.Object,
                _azureSearchIndexRepository.Object,
                _permissionsRepository.Object, 
                _logger.Object);
        }

        [Test]
        public async Task ThenAllReservationsWillBeCollected()
        {
            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _repository.Verify(r => r.GetAllNonLevyForAccountLegalEntity(1), Times.Exactly(2));
        }

        [Test]
        public async Task ThenAllReservationsWillBeIndexed()
        {
            //Arrange
            _permissionsRepository.Setup(r => r.GetAllWithCreateCohortPermission()).Returns(new List<Domain.Entities.ProviderPermission>
            {
                new() {AccountId = 1, AccountLegalEntityId = 2, ProviderId = 1, CanCreateCohort = true}
            });

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _elasticIndexRepository.Verify(x => x.Add(It.Is<IEnumerable<IndexedReservation>>(rIndex =>
                rIndex.Any(r => r.Id.Equals($"1_1_{_expectedReservations.First().Id}")))));
            _elasticIndexRepository.Verify(x => x.Add(It.Is<IEnumerable<IndexedReservation>>(rIndex =>
                rIndex.Any(r => r.Id.Equals($"1_1_{_expectedReservations.Skip(1).First().Id}")))));

        }

        [Test]
        public async Task ThenANewIndexWillBeCreated()
        {
            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _elasticIndexRepository.Verify(r => r.CreateIndex(), Times.Once);
        }

        [Test]
        public async Task Then_The_Old_Indexes_Will_Be_Deleted()
        {
            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _elasticIndexRepository.Verify(r => r.DeleteIndices(5), Times.Once);
        }


        [Test]
        public void ThenIfExceptionIsThrownFromGettingReservationsIndexingWillBeSkipped()
        {
            //Arrange
            _permissionsRepository.Setup(r => r.GetAllWithCreateCohortPermission()).Returns(new List<Domain.Entities.ProviderPermission>
            {
                new() {AccountId = 1, AccountLegalEntityId = 2, ProviderId = 1, CanCreateCohort = true}
            });
            _repository.Setup(x => x.GetAllNonLevyForAccountLegalEntity(2)).Throws(new Exception("Test"));

            //Act
            var action  = () => _service.RefreshReservationIndex();
            action.Should().ThrowAsync<Exception>();

            //Assert
            _elasticIndexRepository.Verify(repo => repo.Add(It.IsAny<IEnumerable<IndexedReservation>>()), Times.Never);
        }

        [Test]
        public void ThenIfExceptionIsThrownFromIndexingThenExceptionIsThrown()
        {
            //Arrange
            _elasticIndexRepository.Setup(x => x.Add(It.IsAny<IEnumerable<IndexedReservation>>()))
                .ThrowsAsync(new Exception("Test"));

            //Act + Assert
            var action = () => _service.RefreshReservationIndex();
            action.Should().ThrowAsync<Exception>();
        }

        [Test]
        public async Task ThenAllLinkedProvidersWillHaveReservationCopiesCreated()
        {
            //Arrange
            var firstReservationId = Guid.NewGuid();
            var secondReservationId = Guid.NewGuid();

            _expectedReservations =
            [
                new() { Id = firstReservationId, AccountId = 1, ProviderId = 1, AccountLegalEntityId = 1 },
                new() { Id = secondReservationId, AccountId = 1, ProviderId = 1, AccountLegalEntityId = 1 }
            ];
            _repository.Setup(x => x.GetAllNonLevyForAccountLegalEntity(1)).Returns(_expectedReservations);

            _permissionsRepository.Setup(r => r.GetAllWithCreateCohortPermission()).Returns(new List<Domain.Entities.ProviderPermission>
            {
                new() {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 1, CanCreateCohort = true},
                new() {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 2, CanCreateCohort = true}
            });

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _elasticIndexRepository.Verify(
                x => x.Add(It.Is<IEnumerable<IndexedReservation>>(rIndex => rIndex.Count().Equals(2))));

            _elasticIndexRepository.Verify(x => x.Add(It.Is<IEnumerable<IndexedReservation>>(rIndex =>
                rIndex.Any(r => r.Id.Equals($"1_1_{firstReservationId}")))));
            _elasticIndexRepository.Verify(x => x.Add(It.Is<IEnumerable<IndexedReservation>>(rIndex =>
                rIndex.Any(r => r.Id.Equals($"2_1_{firstReservationId}")))));
            _elasticIndexRepository.Verify(x => x.Add(It.Is<IEnumerable<IndexedReservation>>(rIndex =>
                rIndex.Any(r => r.Id.Equals($"1_1_{secondReservationId}")))));
            _elasticIndexRepository.Verify(x => x.Add(It.Is<IEnumerable<IndexedReservation>>(rIndex =>
                rIndex.Any(r => r.Id.Equals($"2_1_{secondReservationId}")))));

            
        }

        [Test]
        public async Task ThenAnyProvidersWithPermissionsWillHaveCopiesOfExistingReservationsAdded()
        {
            //Arrange
            var firstReservationId = Guid.NewGuid();

            _expectedReservations =
            [
                new() { Id = firstReservationId, AccountId = 1, ProviderId = 1, AccountLegalEntityId = 1 }
            ];
            _repository.Setup(x => x.GetAllNonLevyForAccountLegalEntity(It.IsAny<long>())).Returns(new List<Reservation>());
            _repository.Setup(x => x.GetAllNonLevyForAccountLegalEntity(1)).Returns(_expectedReservations);

            _permissionsRepository.Setup(r => r.GetAllWithCreateCohortPermission()).Returns(new List<Domain.Entities.ProviderPermission>
            {
                new() {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 1, CanCreateCohort = true},
                new() {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 2, CanCreateCohort = true}
            });

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _elasticIndexRepository.Verify(
                x => x.Add(It.Is<IEnumerable<IndexedReservation>>(rIndex => rIndex.Count().Equals(1))), Times.Exactly(2));

            _elasticIndexRepository.Verify(x => x.Add(It.Is<IEnumerable<IndexedReservation>>(rIndex =>
                rIndex.Any(r =>
                    r.ReservationId.Equals(firstReservationId) &&
                    r.AccountId.Equals(1) &&
                    r.ProviderId.Value.Equals(1) &&
                    r.AccountLegalEntityId.Equals(1)))));

            _elasticIndexRepository.Verify(x => x.Add(It.Is<IEnumerable<IndexedReservation>>(rIndex =>
                rIndex.Any(r =>
                    r.ReservationId.Equals(firstReservationId) &&
                    r.AccountId.Equals(1) &&
                    r.ProviderId.Value.Equals(1) &&
                    r.AccountLegalEntityId.Equals(1)))));
        }

        [Test]
        public async Task Then_The_IndexIs_Still_Created_If_There_Are_No_Matching_Reservations()
        {
            //Arrange
            _expectedReservations = [];
            _repository.Setup(x => x.GetAllNonLevyForAccountLegalEntity(It.IsAny<long>())).Returns(new List<Reservation>());
            _repository.Setup(x => x.GetAllNonLevyForAccountLegalEntity(1)).Returns(_expectedReservations);

            _permissionsRepository.Setup(r => r.GetAllWithCreateCohortPermission()).Returns(new List<Domain.Entities.ProviderPermission>
            {
                new() {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 1, CanCreateCohort = true},
                new() {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 2, CanCreateCohort = true}
            });

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _elasticIndexRepository.Verify(x => x.CreateIndex(), Times.Once());
        }

        [Test]
        public async Task Then_Only_Reservations_For_Provider_With_Permissions_Are_Indexed()
        {
            //Arrange
            var firstReservationId = Guid.NewGuid();
            
            _expectedReservations =
                [new() { Id = firstReservationId, AccountId = 1, ProviderId = 1, AccountLegalEntityId = 1 }];
            _repository.Setup(x => x.GetAllNonLevyForAccountLegalEntity(It.IsAny<long>())).Returns(new List<Reservation>());
            _repository.Setup(x => x.GetAllNonLevyForAccountLegalEntity(1)).Returns(_expectedReservations);

            _permissionsRepository.Setup(r => r.GetAllWithCreateCohortPermission()).Returns(new List<Domain.Entities.ProviderPermission>
            {
                new() {AccountId = 1, AccountLegalEntityId = 1, ProviderId = 1, CanCreateCohort = true},
                new() {AccountId = 1, AccountLegalEntityId = 2, ProviderId = 2, CanCreateCohort = true}
            });

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _elasticIndexRepository.Verify(
                x => x.Add(It.Is<IEnumerable<IndexedReservation>>(rIndex => rIndex.Count().Equals(1))));

            _elasticIndexRepository.Verify(x => x.Add(It.Is<IEnumerable<IndexedReservation>>(rIndex =>
                rIndex.Any(r =>
                    r.ReservationId.Equals(firstReservationId) &&
                    r.AccountId.Equals(1) &&
                    r.ProviderId.Value.Equals(1) &&
                    r.AccountLegalEntityId.Equals(1)))));
            
        }
        
        [Test]
        public async Task ThenIfNoReservationsReturnedIndexingWillBeCreated()
        {
            //Arrange
            _repository.Setup(x => x.GetAllNonLevyForAccountLegalEntity(2)).Returns(new List<Reservation>());

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _elasticIndexRepository.Verify(x => x.CreateIndex(), Times.Once);
            _elasticIndexRepository.Verify(x => x.DeleteIndices(5), Times.Once);
            _elasticIndexRepository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_No_Provider_Permissions_Creates_Empty_Item_Index()
        {
            //Arrange
            var firstReservationId = Guid.NewGuid();
            
            _expectedReservations =
                [new() { Id = firstReservationId, AccountId = 1, ProviderId = 1, AccountLegalEntityId = 1 }];
            _repository.Setup(x => x.GetAllNonLevyForAccountLegalEntity(1)).Returns(_expectedReservations);

            _permissionsRepository.Setup(r => r.GetAllWithCreateCohortPermission()).Returns(new List<Domain.Entities.ProviderPermission>());

            //Act
            await _service.RefreshReservationIndex();

            //Assert
            _elasticIndexRepository.Verify(x => x.CreateIndex(), Times.Once);
            _elasticIndexRepository.Verify(x => x.DeleteIndices(5), Times.Once);
            _elasticIndexRepository.VerifyNoOtherCalls();
        }

    }
}
