using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.RefreshCourses.Services;
using SFA.DAS.Reservations.Domain.ImportTypes;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using Standard = SFA.DAS.Reservations.Domain.ImportTypes.Standard;

namespace SFA.DAS.Reservations.Application.UnitTests.RefreshCourse.Services
{
    public class WhenGettingApprenticeshipCourses
    {
        private ApprenticeshipCoursesService _apprenticeshipCoursesService;
        private Mock<IFindApprenticeshipTrainingService> _standardApiClient;

        [SetUp]
        public void Arrange()
        {
            _standardApiClient = new Mock<IFindApprenticeshipTrainingService>();
            _standardApiClient.Setup(x => x.GetStandards()).ReturnsAsync(new StandardApiResponse
            {
                Standards = new List<Standard>
                    {
                        new Standard
                        {
                            Id = 1,
                            Title = "Some Standard",
                            Level = 1,
                            EffectiveTo = DateTime.Today.AddDays(-1)
                        },
                        new Standard
                        {
                            Id = 2,
                            Title = "Some Standard 2",
                            Level = 1,
                            EffectiveTo = DateTime.Today.AddDays(-1)
                        }
                    }
                }
            );

            _apprenticeshipCoursesService = new ApprenticeshipCoursesService(_standardApiClient.Object);
        }

        [Test]
        public void Then_The_Api_Client_Is_Called_To_Get_Standards()
        {
            //Act
             _apprenticeshipCoursesService.GetCourseInformation();

            //Assert
            _standardApiClient.Verify(x=>x.GetStandards(),Times.Once);
        }

        [Test]
        public void Then_The_List_Of_Mapped_Standards_Is_Returned()
        {
            //Act
            var actual = _apprenticeshipCoursesService.GetCourseInformation();

            //Assert
            Assert.IsInstanceOf<List<Course>>(actual);
            Assert.AreEqual(2, actual.Count);
        }
    }
}
