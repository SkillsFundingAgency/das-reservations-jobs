using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.Reservations.AcceptanceTests.Steps
{
    [Binding]
    public class ReservationConfirmedSteps
    {
        [Given(@"I have a reservation")]
        public void GivenIHaveAReservation()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"a confirm reservation event is triggered")]
        public void WhenAConfirmReservationEventIsTriggered()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the reservation status will be (.*)")]
        public void ThenTheReservationStatusWillBeConfirmed(string status)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
