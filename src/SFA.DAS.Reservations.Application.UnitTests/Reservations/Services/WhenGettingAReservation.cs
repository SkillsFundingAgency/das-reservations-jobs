using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services;

public class WhenGettingAReservation
{
    [Test, MoqAutoData]
    public void ThenWillThrowExceptionIfReservationIdIsInvalid(ReservationService service)
    {
        //Arrange
        var reservationId = Guid.Empty;

        //Act
        Assert.ThrowsAsync<ArgumentException>(() => service.GetReservation(reservationId));
    }

    [Test, MoqAutoData]
    public async Task ThenIfNoReservationFoundForAGivenIdReturnNull(
        Guid reservationId,
        [Frozen] Mock<IReservationRepository> repository,
        [Frozen] Mock<ILogger> logger,
        ReservationService service)
    {
        //Arrange
        Reservation reservation = null;

        repository
            .Setup(x => x.GetReservationById(It.IsAny<Guid>()))
            .ReturnsAsync(reservation);

        //Act
        var result = await service.GetReservation(reservationId);

        //Assert
        result.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task ThenShouldReturnReservation_WithNonLevyPropertiesSet(
        Guid reservationId,
        Reservation reservation,
        [Frozen] Mock<IReservationRepository> repository,
        ReservationService service
    )
    {
        // Arrange
        repository
            .Setup(x => x.GetReservationById(It.IsAny<Guid>()))
            .ReturnsAsync(reservation);

        //Act
        var result = await service.GetReservation(reservationId);

        //Assert
        result.CourseName.Should().Be(reservation.Course.Title);
        result.CourseLevel.Should().Be(reservation.Course.Level);
    }

    [Test, MoqAutoData]
    public async Task ThenShouldReturnReservation_WithoutNonLevyPropertiesSet(
        Guid reservationId,
        Reservation reservation,
        [Frozen] Mock<IReservationRepository> repository,
        ReservationService service
    )
    {
        // Arrange
        UnsetUnusedLevyReservationProperties(reservation);

        repository
            .Setup(x => x.GetReservationById(It.IsAny<Guid>()))
            .ReturnsAsync(reservation);

        //Act
        var result = await service.GetReservation(reservationId);

        //Assert
        result.CourseName.Should().Be(null);
        result.CourseLevel.Should().Be(null);
    }

    private static void UnsetUnusedLevyReservationProperties(Reservation reservation)
    {
        reservation.Course = null;
        reservation.CourseId = null;
    }
}