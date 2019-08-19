using System;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Jobs.Jobs
{
    public class CheckQueueHealthJob
    {
        private readonly ICheckAvailableQueuesHealth _queueHealth;

        public CheckQueueHealthJob(ICheckAvailableQueuesHealth queueHealth)
        {
            _queueHealth = queueHealth;
        }

        public void Run([TimerTrigger("*/20 * * * * *", RunOnStartup = true)] TimerInfo timer)
        {
            _queueHealth.Handle().Wait();
            Console.WriteLine($"Will run again at {timer.FormatNextOccurrences(1)} ");
        }
    }
}