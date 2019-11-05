using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Data.UnitTests.ProviderPermissionRepository
{
    public class WhenGettingAllProvidersForAccountLegalEntity
    {
        [Test, MoqAutoData]
        public void Then_Filters_By_AccountLegalEntity(
            long accountLegalEntityId,
            List<ProviderPermission> permissions,
            [Frozen] Mock<IReservationsDataContext> mockContext,
            Data.Repository.ProviderPermissionRepository repository)
        {
            permissions[0].AccountLegalEntityId = accountLegalEntityId;
            mockContext
                .Setup(context => context.ProviderPermissions)
                .ReturnsDbSet(permissions);

            var result = repository.GetAllForAccountLegalEntity(accountLegalEntityId);

            result.Count().Should().Be(1);
            result.First().Should().BeEquivalentTo(permissions[0]);
        }
    }
}