namespace SFA.DAS.Reservations.Domain.Accounts;

public class UserDetails
{
    public string UserRef { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool CanReceiveNotifications { get; set; }
    public byte Status { get; set; }
}