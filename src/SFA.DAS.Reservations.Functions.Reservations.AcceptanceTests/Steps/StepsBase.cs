using System;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests.Steps
{
    public class StepsBase
    {
        protected readonly IServiceProvider Services;
        protected readonly TestData TestData;

        public StepsBase(TestServiceProvider serviceProvider, TestData testData)
        {
            Services = serviceProvider;
            TestData = testData;
        }
    }
}
