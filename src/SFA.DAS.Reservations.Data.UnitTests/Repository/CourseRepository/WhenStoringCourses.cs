using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.CourseRepository
{
    public class WhenStoringCourses
    {
        private Data.Repository.CourseRepository _courseRepository;
        private Mock<IReservationsDataContext> _dataContext;
        private Mock<DatabaseFacade> _dataFacade;
        private Mock<DbContext> _dbContext;
        private Mock<IDbContextTransaction> _dbContextTransaction;

        [SetUp]
        public void Arrange()
        {
            var expectedCourse = new Course
            {
                CourseId = "2-2",
                Title = "First Title",
                Level = 2,
                EffectiveTo = DateTime.Today.AddDays(-1)
            };

            _dbContextTransaction = new Mock<IDbContextTransaction>();
            _dbContext = new Mock<DbContext>();
            _dataContext = new Mock<IReservationsDataContext>();
            _dataFacade = new Mock<DatabaseFacade>(_dbContext.Object);
            _dataFacade.Setup(x => x.BeginTransaction()).Returns(_dbContextTransaction.Object);
            _dataContext.Setup(x => x.Apprenticeships).ReturnsDbSet(new List<Course>
            {
                expectedCourse
            });
            _dataContext.Setup(x => x.Apprenticeships.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityEntry<Course>) null);
            _dataContext.Setup(x => x.Apprenticeships.FindAsync(expectedCourse.CourseId))
                .ReturnsAsync(expectedCourse);

            _dataContext.Setup(x => x.Database)
                .Returns(_dataFacade.Object);

            _courseRepository = new Data.Repository.CourseRepository(_dataContext.Object);
        }

        [Test]
        public async Task Then_The_Course_Is_Added_To_The_Repository()
        {
            //Arrange
            var expectedCourse = new Course
            {
                CourseId="1",
                Title = "Title1",
                Level = 1,
                EffectiveTo = DateTime.Today
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

        [Test]
        public async Task Then_If_The_Course_Exists_It_Is_Updated()
        {
            //Arrange
            var expectedCourse = new Course
            {
                CourseId = "2-2",
                Title = "Title",
                Level = 1,
                EffectiveTo = DateTime.Today
            };

            //Act
            await _courseRepository.Add(expectedCourse);

            //Assert
            _dataContext.Verify(x => x.Apprenticeships.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Never);
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
        }
    }
}
