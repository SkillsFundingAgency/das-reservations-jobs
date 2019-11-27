using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests.Steps
{
    [Binding]
    public class ReservationDeletedSteps
    {
        [Given(@"I have a reservation ready for deletion")]
        public void GivenIHaveAReservationReadyForDeletion()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"a delete reservation event is triggered")]
        public void WhenADeleteReservationEventIsTriggered()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the reservation search index should be updated with the deleted reservation removed")]
        public void ThenTheReservationSearchIndexShouldBeUpdatedWithTheDeletedReservationRemoved()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
