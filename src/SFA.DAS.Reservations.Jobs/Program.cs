using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Jobs.Services;

namespace SFA.DAS.Reservations.Jobs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            var job = serviceProvider.GetService<IJob>();
            job.Execute();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAzureSearchHelper, AzureSearchHelper>();
            services.AddTransient<IAzureSearchReservationIndexRepository, AzureSearchReservationIndexRepository>();
        }
    }
} 