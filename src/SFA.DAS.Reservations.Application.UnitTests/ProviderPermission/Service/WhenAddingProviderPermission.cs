using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.Reservations.Application.ProviderPermissions.Service;
using SFA.DAS.Reservations.Application.UnitTests.Customisations;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.ProviderPermission.Service
{
    public class WhenAddingProviderPermission
    {
        private ProviderPermissionService _service;
        private Mock<IProviderPermissionRepository> _repo;
        private Mock<ILogger<ProviderPermissionService>> _logger;
        private Mock<IReservationService> _reservationIndexService;
        private Mock<IUpdatedPermissionsEventValidator> _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IUpdatedPermissionsEventValidator>();
            _validator
                .Setup(validator => validator.Validate(It.IsAny<UpdatedPermissionsEvent>()))
                .Returns(true);
            _repo = new Mock<IProviderPermissionRepository>();
            _logger = new Mock<ILogger<ProviderPermissionService>>();
            _reservationIndexService = new Mock<IReservationService>();
            _service = new ProviderPermissionService(_repo.Object, _logger.Object, _reservationIndexService.Object);
        }

        [Test]
        public async Task ThenCallsRepositoryToAddPermissionToDatabase_And_Does_Not_Call_Delete_On_Index()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                10, 11, 12,
                13, 14, Guid.NewGuid(),
                "test@example.com", "Test",
                "Tester", new HashSet<Operation> {Operation.CreateCohort},null, DateTime.Now);

            //Act
            await _service.AddProviderPermission(permissionEvent);

            //Assert
            _repo.Verify(s => s.Add(It.Is<Domain.Entities.ProviderPermission>(p =>
                p.AccountId.Equals(permissionEvent.AccountId) &&
                p.AccountLegalEntityId.Equals(permissionEvent.AccountLegalEntityId) &&
                p.ProviderId.Equals(permissionEvent.Ukprn) &&
                p.CanCreateCohort)), Times.Once);
            _reservationIndexService.Verify(x => x.DeleteProviderFromSearchIndex(It.IsAny<uint>(),It.IsAny<long>()), Times.Never);
        }

        [Test]
        public async Task ThenSetsCanCreateCohortToFalseIfNoPermissionFound_And_Updates_Index()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                10, 11, 12,
                13, 14, Guid.NewGuid(),
                "test@example.com", "Test",
                "Tester", new HashSet<Operation> {Operation.Recruitment}, null, DateTime.Now);

            //Act
            await _service.AddProviderPermission(permissionEvent);

            //Assert
            _repo.Verify(s => s.Add(It.Is<Domain.Entities.ProviderPermission>(p =>
                !p.CanCreateCohort)), Times.Once);
            _reservationIndexService.Verify(
                x => x.DeleteProviderFromSearchIndex(Convert.ToUInt32(permissionEvent.Ukprn),
                    permissionEvent.AccountLegalEntityId), Times.Once);
        }

        [Test]
        public async Task ThenSetsCanCreateCohortToFalseIfPermissionIsNull_And_Updates_Index()
        {
            //Arrange
            var permissionEvent = new UpdatedPermissionsEvent(
                10, 11, 12,
                13, 14, Guid.NewGuid(),
                "test@example.com", "Test",
                "Tester", null, null, DateTime.Now);

            //Act
            await _service.AddProviderPermission(permissionEvent);

            //Assert
            _repo.Verify(s => s.Add(It.Is<Domain.Entities.ProviderPermission>(p =>
                !p.CanCreateCohort)), Times.Once);
            _reservationIndexService.Verify(
                x => x.DeleteProviderFromSearchIndex(Convert.ToUInt32(permissionEvent.Ukprn),
                    permissionEvent.AccountLegalEntityId), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task And_CanCreateCohort_Then_Adds_Provider_To_Search_Index(
            [ArrangeUpdatedPermissionsEvent(Operation = Operation.CreateCohort)]
            UpdatedPermissionsEvent updatedEvent,
            [Frozen] Mock<IReservationService> mockReservationsService,
            ProviderPermissionService service)
        {
            await service.AddProviderPermission(updatedEvent);

            mockReservationsService.Verify(reservationService =>
                    reservationService.AddProviderToSearchIndex((uint) updatedEvent.Ukprn,
                        updatedEvent.AccountLegalEntityId),
                Times.Once);
        }
    }
}