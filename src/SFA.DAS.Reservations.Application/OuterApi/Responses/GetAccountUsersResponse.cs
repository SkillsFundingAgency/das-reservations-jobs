using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SFA.DAS.Reservations.Application.OuterApi.Responses;

public record GetAccountUsersResponse
{
    public ICollection<AccountUser> AccountUsers { get; set; }
}

public record AccountUser
{
    public string UserRef { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool CanReceiveNotifications { get; set; }
}