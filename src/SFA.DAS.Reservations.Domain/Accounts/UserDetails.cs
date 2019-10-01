using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.Reservations.Domain.Accounts
{
    public class UserDetails
    {
        public string UserRef { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool CanReceiveNotifications { get; set; }
        public byte Status { get; set; }

        public static implicit operator UserDetails(TeamMemberViewModel source)
        {
            return new UserDetails
            {
                UserRef = source.UserRef,
                Name = source.Name,
                Email = source.Email,
                Role = source.Role,
                CanReceiveNotifications = source.CanReceiveNotifications,
                Status = (byte)source.Status
            };
        }
    }
}