namespace SFA.DAS.Reservations.Application.OuterApi.Requests;

public class GetAccountUsersRequest(long accountId) : IGetApiRequest
{
    public string GetUrl => $"accounts/{accountId}/users";
}