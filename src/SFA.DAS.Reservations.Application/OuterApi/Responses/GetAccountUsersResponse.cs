using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SFA.DAS.Reservations.Application.OuterApi.Responses;

public enum InvitationStatus : byte
{
    [Description("Invitation awaiting response")] Pending = 1,
    [Description("Active")] Accepted = 2,
    [Description("Invitation expired")] Expired = 3,
    [Description("Invitation cancelled")] Deleted = 4,
}

public record GetAccountUsersResponse
{
    public ICollection<AccountUser> AccountUsers { get; set; }
}

public record AccountUser
{
    public string UserRef { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool CanReceiveNotifications { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public InvitationStatus Status { get; set; }
}