using System;
using Microsoft.Extensions.Hosting;

namespace SFA.DAS.Reservations.Jobs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = new HostBuilder();
            try
            {
                hostBuilder
                    .ConfigureWebJobs(b =>
                    {
                        b.AddAzureStorageCoreServices()
                            .AddTimers();
                    })
                    .ConfigureServices(AppStart.Configuration.AddServiceConfiguration)
                    .UseConsoleLifetime();


                using (var host = hostBuilder.Build())
                {
                     host.RunAsync().Wait();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
