﻿using System;
using System.Collections.Generic;
using AutoFixture;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.Reservations.Application.UnitTests.Customisations
{
    public class ArrangeUpdatedPermissionsEventCustomisation : ICustomization
    {
        private readonly Operation _operation;

        public ArrangeUpdatedPermissionsEventCustomisation(Operation operation)
        {
            _operation = operation;
        }

        public void Customize(IFixture fixture)
        {
            var permissionEvent = new UpdatedPermissionsEvent(
                10, 11, 12,
                13, 14, Guid.NewGuid(),
                "test@example.com", "Test",
                "Tester", [_operation], null, DateTime.Now);
            fixture.Inject(permissionEvent);
        }
    }
}