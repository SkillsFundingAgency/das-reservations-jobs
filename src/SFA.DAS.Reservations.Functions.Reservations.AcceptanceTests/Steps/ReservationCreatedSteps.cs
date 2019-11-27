using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests.Steps
{
    [Binding]
    public class ReservationCreatedSteps
    {
        [Given(@"I have a reservation ready for creation")]
        public void GivenIHaveAReservationReadyForCreation()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"a create reservation event is triggered")]
        public void WhenACreateReservationEventIsTriggered()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the reservation search index should be updated")]
        public void ThenTheReservationSearchIndexShouldBeUpdated()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
