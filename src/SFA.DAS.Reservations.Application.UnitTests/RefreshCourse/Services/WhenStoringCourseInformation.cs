using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.RefreshCourses.Services;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.UnitTests.RefreshCourse.Services
{
    public class WhenStoringCourseInformation
    {
        private CourseService _service;
        private Mock<ICourseRepository> _repository;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<ICourseRepository>();

            _service = new CourseService(_repository.Object);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_With_The_Mapped_Entity()
        {
            //Arrange
            var course = new Course(1,"My Course", 1, DateTime.Today);
            
            //Act
            await _service.Store(course);

            //Act
            _repository.Verify(x=>x.Add(It.Is<Domain.Entities.Course>(
                c=>c.CourseId.Equals(course.Id) &&
                   c.Level.Equals(course.Level) &&
                   c.Title.Equals(course.Title) &&
                   c.EffectiveTo.Equals(course.EffectiveTo) &&
                   c.ApprenticeshipType.Equals(course.ApprenticeshipType))), Times.Once);
        }
  
        [Test]
        public async Task Then_The_Repository_Is_Called_With_The_Mapped_Entity_And_Sets_EffectiveTo_To_Null_If_DateTime_Min()
        {
            //Arrange
            var course = new Course(1,"My Course", 1, DateTime.MinValue, "Apprenticeship");
            
            //Act
            await _service.Store(course);

            //Act
            _repository.Verify(x=>x.Add(It.Is<Domain.Entities.Course>(
                c=>c.CourseId.Equals(course.Id) &&
                   c.Level.Equals(course.Level) &&
                   c.Title.Equals(course.Title) &&
                   c.EffectiveTo.Equals(null))), Times.Once);
        }


        
    }
}
