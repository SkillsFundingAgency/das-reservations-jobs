namespace SFA.DAS.Reservations.Domain.Configuration
{
    public class ReservationJobsEnvironment
    {
        public virtual string EnvironmentName { get; }

        public ReservationJobsEnvironment(string environmentName)
        {
            EnvironmentName = environmentName.ToLower();
        }
    }
}