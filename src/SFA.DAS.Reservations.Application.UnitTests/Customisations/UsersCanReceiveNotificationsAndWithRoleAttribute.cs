using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using AutoFixture.NUnit3;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.UnitTests.Customisations
{
    public class UsersCanReceiveNotificationsAndWithRoleAttribute : CustomizeAttribute
    {
        public string Role { get; set; }

        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameter.ParameterType != typeof(List<UserDetails>))
            {
                throw new ArgumentException(nameof(parameter));
            }

            return new UsersCanReceiveNotificationsAndWithRoleCustomisation(Role);
        }
    }
}