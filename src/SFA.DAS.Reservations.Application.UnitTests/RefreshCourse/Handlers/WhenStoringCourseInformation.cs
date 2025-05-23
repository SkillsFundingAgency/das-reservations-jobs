﻿using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.RefreshCourses.Handlers;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.UnitTests.RefreshCourse.Handlers
{
    public class WhenStoringCourseInformation
    {
        private StoreCourseHandler _handler;
        private Mock<ICourseService> _service;

        [SetUp]
        public void Arrange()
        {
            _service = new Mock<ICourseService>();
            _handler = new StoreCourseHandler(_service.Object);
        }

        [Test]
        public async Task Then_The_Service_Is_Called_With_The_Course()
        {
            //Arrange
            var course = new Course(1,"Test",3, DateTime.Today, "Apprenticeship");

            //Act
            await _handler.Handle(course);
        }
    }
}
