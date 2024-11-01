﻿using Microsoft.Extensions.Configuration;
using System.IO;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Reservations.Functions.RefreshCourse;

public static class ConfigurationExtensions
{
    public static IConfiguration BuildDasConfiguration(this IConfigurationBuilder configBuilder)
    {
        configBuilder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();

        configBuilder.AddJsonFile("local.settings.json", optional: true);

        var config = configBuilder.Build();

        configBuilder.AddAzureTableStorage(options =>
        {
#if DEBUG
            options.ConfigurationKeys = config["Values:ConfigNames"].Split(",");
            options.StorageConnectionString = config["Values:ConfigurationStorageConnectionString"];
            options.EnvironmentName = config["Values:EnvironmentName"];
#else
            options.ConfigurationKeys = config["ConfigNames"].Split(",");
            options.StorageConnectionString = config["ConfigurationStorageConnectionString"];
            options.EnvironmentName = config["EnvironmentName"];
#endif

            options.PreFixConfigurationKeys = false;
        });

        return configBuilder.Build();
    }
}