namespace SFA.DAS.Reservations.Application.OuterApi.Requests;

public record GetProviderRequest(uint ukPrn) : IGetApiRequest
{
    public string GetUrl => $"providers/{ukPrn}";
}