using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using AutoFixture.NUnit3;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.UnitTests.Customisations
{
    public class UsersWithRoleAttribute : CustomizeAttribute
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

            return new ArrangeUsersWithRoleCustomisation(Role);
        }
    }

    public class ArrangeUsersWithRoleCustomisation : ICustomization
    {
        private readonly string _role;

        public ArrangeUsersWithRoleCustomisation(string role)
        {
            _role = role;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<UserDetails>(composer => composer
                .With(user => user.Role, _role));
        }
    }
}