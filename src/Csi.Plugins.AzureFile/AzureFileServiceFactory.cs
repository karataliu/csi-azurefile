using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Csi.Plugins.AzureFile
{
    sealed class AzureFileServiceFactory : IAzureFileServiceFactory
    {
        private readonly ILoggerFactory loggerFactory;

        public AzureFileServiceFactory(ILoggerFactory loggerFactory) => this.loggerFactory = loggerFactory;

        public IAzureFileService Create(AzureFileAccount azureFileAccount)
        {
            var sc = new StorageCredentials(azureFileAccount.Name, azureFileAccount.Key);
            var csa = new CloudStorageAccount(
                sc,
                AzureEnvironmentHelper.GetStorageEndpointSuffix(azureFileAccount.EnvironmentName),
                true);

            return new AzureFileService(csa, loggerFactory.CreateLogger<AzureFileService>());
        }
    }

    sealed class AzureFileAccount
    {
        public string EnvironmentName { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
    }
}
