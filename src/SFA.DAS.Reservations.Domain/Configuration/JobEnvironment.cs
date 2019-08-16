using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Domain.Configuration
{
    public class JobEnvironment
    {
        public string EnvironmentName { get; }

        public JobEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }
    }
}
