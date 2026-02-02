namespace SFA.DAS.Reservations.Application.OuterApi.Requests;

public record GetCoursesRequest : IGetApiRequest
{
    public string GetUrl => "trainingcourses";
}
