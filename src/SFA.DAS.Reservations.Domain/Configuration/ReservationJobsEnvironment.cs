namespace SFA.DAS.Reservations.Domain.Configuration
{
    public class ReservationJobsEnvironment(string environmentName)
    {
        public virtual string EnvironmentName { get; } = environmentName.ToLower();
    }
}