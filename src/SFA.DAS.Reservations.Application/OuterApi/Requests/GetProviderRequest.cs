namespace SFA.DAS.Reservations.Application.OuterApi.Requests;

public record GetProviderRequest(uint UkPrn) : IGetApiRequest
{
    public string GetUrl => $"providers/{UkPrn}";
}