namespace SFA.DAS.Reservations.Application.OuterApi.Requests;

public record GetStandardsRequest : IGetApiRequest
{
    public string GetUrl => "trainingcourses";
}