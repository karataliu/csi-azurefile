using System;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace Csi.AzureFile
{
    class AzureFileServiceFactory : IAzureFileServiceFactory
    {
        private readonly ILoggerFactory loggerFactory;

        public AzureFileServiceFactory(ILoggerFactory loggerFactory) => this.loggerFactory = loggerFactory;

        public IAzureFileService Create()
        {
            var conn = Environment.GetEnvironmentVariable("CONN");
            var csa = CloudStorageAccount.Parse(conn);

            return new AzureFileService(csa, loggerFactory.CreateLogger<AzureFileService>());
        }
    }
}
