using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.NUnit3;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.UnitTests.Customisations
{
    public class ArrangeUpdatedPermissionsEventAttribute : CustomizeAttribute
    {
        public Operation Operation { get; set; }

        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameter.ParameterType != typeof(UpdatedPermissionsEvent))
            {
                throw new ArgumentException(nameof(parameter));
            }

            return new ArrangeUpdatedPermissionsEventCustomisation(Operation);
        }
    }
}