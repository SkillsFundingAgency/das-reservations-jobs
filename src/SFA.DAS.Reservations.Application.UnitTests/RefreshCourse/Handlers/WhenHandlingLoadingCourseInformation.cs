using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.RefreshCourses.Handlers;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.UnitTests.RefreshCourse.Handlers;

public class WhenHandlingLoadingCourseInformation
{
    private GetCoursesHandler _handler;
    private Mock<IApprenticeshipCourseService> _service;

    [SetUp]
    public void Arrange()
    {
        _service = new Mock<IApprenticeshipCourseService>();
        _service.Setup(x => x.GetCourseInformation()).Returns(new List<Course> {new Course(1, "", 1, DateTime.Today)});
        _handler = new GetCoursesHandler(_service.Object);
    }

    [Test]
    public void Then_The_Api_Is_Called_To_Retrieve_Course_Information()
    {
        _handler.Handle();
        _service.Verify(x=>x.GetCourseInformation(), Times.Once);
    }

    [Test]
    public void Then_The_Course_Data_Is_Returned_To_Be_Added_To_The_Queue()
    {
        var actual = _handler.Handle();

        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<List<Course>>();
    }
}