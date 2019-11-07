using System;
using Microsoft.Extensions.Logging;
using Moq;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace SFA.DAS.Reservations.Application.UnitTests.Extensions
{
    public static class MockILoggerExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level)
        {
            logger.Verify(l => l.Log(level, It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
        }
    }
}
