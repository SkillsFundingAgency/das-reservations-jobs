using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.CourseRepository
{
    public class WhenStoringCourses
    {
        private Repository.CourseRepository _courseRepository;
        private Mock<IReservationsDataContext> _dataContext;

        [SetUp]
        public void Arrange()
        {
            _dataContext = new Mock<IReservationsDataContext>();
            _dataContext.Setup(x => x.Apprenticeships).ReturnsDbSet(new List<Course>());
            _dataContext.Setup(x => x.Apprenticeships.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityEntry<Course>) null);
            _courseRepository = new Repository.CourseRepository(_dataContext.Object);
        }

        [Test]
        public async Task Then_The_Course_Is_Added_To_The_Repository()
        {
            //Arrange
            var expectedCourse = new Course
            {
                CourseId="1",
                Title = "Title1",
                Level = 1
            };

            //Act
            await _courseRepository.Add(expectedCourse);

            //Assert 
            _dataContext.Verify(x=>x.Apprenticeships.AddAsync(It.Is<Course>(
                c => c.CourseId.Equals(expectedCourse.CourseId) &&
                     c.Title.Equals(expectedCourse.Title) &&
                     c.Level.Equals(expectedCourse.Level)
                     ),It.IsAny<CancellationToken>()),Times.Once);
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);

        }
    }
}
