﻿using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Util.Extensions.Logging.Step;

namespace Csi.AzureFile
{
    sealed class AzureFileService : IAzureFileService
    {
        private readonly CloudStorageAccount cloudStorageAccount;
        private readonly CloudFileClient cloudFileClient;
        private readonly ILogger logger;

        public AzureFileService(CloudStorageAccount cloudStorageAccount, ILogger<AzureFileService> logger)
        {
            this.cloudStorageAccount = cloudStorageAccount;
            this.cloudFileClient = cloudStorageAccount.CreateCloudFileClient();
            this.logger = logger;
        }

        public async Task<CloudFileShare> CreateShareAsync(string shareName, int? quota)
        {
            using (var _s = logger.StepInformation("{0}: shareName:{1}, quota:{2}",
                nameof(CreateShareAsync), shareName, quota))
            {
                var share = cloudFileClient.GetShareReference(shareName);
                share.Properties.Quota = quota;
                await share.CreateAsync();
                await share.FetchAttributesAsync();

                _s.Commit();
                return share;
            }
        }

        public async Task DeleteShareAsync(string shareName)
        {
            using (var _s = logger.StepInformation("{0}: shareName:{1}", nameof(DeleteShareAsync), shareName))
            {
                var share = cloudFileClient.GetShareReference(shareName);
                await share.DeleteIfExistsAsync();

                _s.Commit();
            }
        }

        public SmbShareCredential GetShareCredential()
        {
            return new SmbShareCredential {
                Username = cloudStorageAccount.Credentials.AccountName,
                Password = cloudStorageAccount.Credentials.ExportBase64EncodedKey(),
            };
        }

        public string GetShareUnc(string shareName)
            => $"\\\\{cloudStorageAccount.FileEndpoint.Host}\\{shareName}";
    }
}
