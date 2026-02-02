using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.RefreshCourses.Services;
using SFA.DAS.Reservations.Domain.ImportTypes;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.UnitTests.RefreshCourse.Services;

public class WhenGettingApprenticeshipCourses
{
    private ApprenticeshipCoursesService _apprenticeshipCoursesService;
    private Mock<IReferenceDataImportService> _courseApiClient;

    [SetUp]
    public void Arrange()
    {
        _courseApiClient = new Mock<IReferenceDataImportService>();
        _courseApiClient.Setup(x => x.GetCourses()).ReturnsAsync(new CourseApiResponse
            {
                Courses =
                [
                    new CourseApiResponseItem
                    {
                        Id = "1",
                        Title = "Some Course",
                        Level = 1,
                        EffectiveTo = DateTime.Today.AddDays(-1),
                        ApprenticeshipType = "Foundation",
                        LearningType = "Standard"
                    },
                    new CourseApiResponseItem
                    {
                        Id = "2",
                        Title = "Some Course 2",
                        Level = 1,
                        EffectiveTo = DateTime.Today.AddDays(-1),
                        ApprenticeshipType = "OtherType",
                        LearningType = "Standard"
                    }
                ]
            }
        );

        _apprenticeshipCoursesService = new ApprenticeshipCoursesService(_courseApiClient.Object);
    }

    [Test]
    public void Then_The_Api_Client_Is_Called_To_Get_Courses()
    {
        _apprenticeshipCoursesService.GetCourseInformation();

        _courseApiClient.Verify(x => x.GetCourses(), Times.Once);
    }

    [Test]
    public void Then_The_List_Of_Mapped_Courses_Is_Returned()
    {
        //Act
        var actual = _apprenticeshipCoursesService.GetCourseInformation();

        //Assert
        actual.Should().BeOfType<List<Course>>();
        actual.Count.Should().Be(2);
    }
}