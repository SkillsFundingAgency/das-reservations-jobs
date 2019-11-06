using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Reservations.Application.RefreshCourses.Services;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.UnitTests.RefreshCourse.Services
{
    public class WhenGettingApprenticeshipCourses
    {
        private ApprenticeshipCoursesService _apprenticeshipCoursesService;
        private Mock<IStandardApiClient> _standardApiClient;

        [SetUp]
        public void Arrange()
        {
            _standardApiClient = new Mock<IStandardApiClient>();
            _standardApiClient.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<StandardSummary>
            {
                new StandardSummary
                {
                    Id = "1",
                    Title = "Some Standard",
                    Level = 1,
                    IsActiveStandard = true,
                    EffectiveTo = DateTime.Today.AddDays(-1)
                },
                new StandardSummary
                {
                    Id = "2",
                    Title = "Some Standard 2",
                    Level = 1,
                    IsActiveStandard = false,
                    EffectiveTo = DateTime.Today.AddDays(-1)
                }
            });

            _apprenticeshipCoursesService = new ApprenticeshipCoursesService(_standardApiClient.Object);
        }

        [Test]
        public void Then_The_Api_Client_Is_Called_To_Get_Standards()
        {
            //Act
             _apprenticeshipCoursesService.GetCourseInformation();

            //Assert
            _standardApiClient.Verify(x=>x.GetAllAsync(),Times.Once);
        }

        [Test]
        public void Then_Inactive_Courses_Are_Not_Mapped()
        {
            //Act
            var actual = _apprenticeshipCoursesService.GetCourseInformation();

            //Assert
            Assert.IsNull(actual.FirstOrDefault(c=>c.Id.Equals("8-8-8")));
            Assert.IsNull(actual.FirstOrDefault(c=>c.Id.Equals("2")));
        }
        
        [Test]
        public void Then_The_List_Of_Mapped_Standards_Is_Returned()
        {
            //Act
            var actual = _apprenticeshipCoursesService.GetCourseInformation();

            //Assert
            Assert.IsInstanceOf<List<Course>>(actual);
            Assert.AreEqual(1, actual.Count);
        }
    }
}
