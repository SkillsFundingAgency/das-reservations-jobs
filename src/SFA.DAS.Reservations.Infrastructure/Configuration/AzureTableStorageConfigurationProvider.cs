﻿using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.Configuration
{
    public class AzureTableStorageConfigurationProvider : ConfigurationProvider
    {
        private readonly string[] _configName;
        private readonly string _environment;
        private readonly string _version;
        private readonly CloudStorageAccount _storageAccount;

        public AzureTableStorageConfigurationProvider(string connection, string[] configName, string environment, string version)
        {
            _configName = configName;
            _environment = environment;
            _version = version;
            _storageAccount = CloudStorageAccount.Parse(connection);
        }

        public override void Load()
        {
            var table = GetTable();
            foreach (var config in _configName)
            {
                var configParams = config.Split(':');
                var configDefaultSectionName = configParams.Length > 1 ? configParams[1] : string.Empty;

                var operation = GetOperation(configParams[0], _environment, _version);
                var result = table.ExecuteAsync(operation).Result;

                var configItem = (ConfigurationItem)result.Result;

                var data = new StorageConfigParser().ParseConfig(configItem, configDefaultSectionName);
                data.ToList().ForEach(x => Data.Add(x.Key, x.Value));
            }

        }

        private CloudTable GetTable()
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference("Configuration");
        }

        private TableOperation GetOperation(string serviceName, string environmentName, string version)
        {
            return TableOperation.Retrieve<ConfigurationItem>(environmentName, $"{serviceName}_{version}");
        }
    }
}