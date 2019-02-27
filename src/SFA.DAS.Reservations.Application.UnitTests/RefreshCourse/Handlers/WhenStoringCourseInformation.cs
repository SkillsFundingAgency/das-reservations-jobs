using System;
using System.Collections.Generic;
using System.Text;
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
            _handler = new StoreCourseHandler();
        }

        [Test]
        public async Task Then_The_Service_Is_Called_With_The_Course()
        {

        }
    }
}
