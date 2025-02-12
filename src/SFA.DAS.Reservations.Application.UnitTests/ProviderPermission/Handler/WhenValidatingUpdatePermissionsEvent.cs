using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.Reservations.Application.ProviderPermissions.Handlers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.ProviderPermission.Handler
{
    public class WhenValidatingUpdatePermissionsEvent
    {
        [Test, MoqAutoData]
        public void And_No_AccountId_Then_Not_Valid(
            UpdatedPermissionsEventValidator validator)	
        {	
            //Arrange	
            var permissionEvent = new UpdatedPermissionsEvent(	
                0, 11, 12,	
                13, 14, Guid.NewGuid(),	
                "test@example.com", "Test",	
                "Tester", [Operation.Recruitment], null, DateTime.Now);	

            //Act	
            var result = validator.Validate(permissionEvent);

            //Assert	
            result.Should().BeFalse();
        }

        [Test, MoqAutoData]
        public void And_No_AccountLegalEntity_Then_Not_Valid(
            UpdatedPermissionsEventValidator validator)
        {	
            //Arrange	
            var permissionEvent = new UpdatedPermissionsEvent(	
                1, 0, 12,	
                13, 14, Guid.NewGuid(),	
                "test@example.com", "Test",	
                "Tester", [Operation.Recruitment], null, DateTime.Now);	

            //Act	
            var result = validator.Validate(permissionEvent);

            //Assert	
            result.Should().BeFalse();	
        }

        [Test, MoqAutoData]
        public void And_No_Ukprn_Then_Not_Valid(
            UpdatedPermissionsEventValidator validator)	
        {	
            //Arrange	
            var permissionEvent = new UpdatedPermissionsEvent(	
                1, 11, 12,	
                13, 0, Guid.NewGuid(),	
                "test@example.com", "Test",	
                "Tester", [Operation.Recruitment], null, DateTime.Now);	

            //Act	
            var result = validator.Validate(permissionEvent);

            //Assert	
            result.Should().BeFalse();
        }

        [Test, MoqAutoData]
        public void And_All_Required_Fields_Then_Is_Valid(
            UpdatedPermissionsEvent permissionsEvent,
            UpdatedPermissionsEventValidator validator)	
        {
            //Act	
            var result = validator.Validate(permissionsEvent);

            //Assert	
            result.Should().BeTrue();
        }
    }
}