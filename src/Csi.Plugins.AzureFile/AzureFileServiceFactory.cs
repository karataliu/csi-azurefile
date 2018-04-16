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
            var csa = new CloudStorageAccount(
                new StorageCredentials(azureFileAccount.Id.Name, azureFileAccount.Key),
                AzureEnvironmentHelper.GetStorageEndpointSuffix(azureFileAccount.Id.EnvironmentName),
                true);

            return new AzureFileService(csa, loggerFactory.CreateLogger<AzureFileService>());
        }
    }

    sealed class AzureFileAccount
    {
        public AzureFileAccountId Id { get; set; }
        public string Key { get; set; }
    }

    sealed class AzureFileAccountId
    {
        public string EnvironmentName { get; set; }
        public string Name { get; set; }
    }
}
