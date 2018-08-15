using System;
using Csi.Helpers.Azure;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.File;

namespace Csi.Plugins.AzureFile.Tests.Scenarios.K8s
{
    public class TestAzureFile
    {
        private readonly CloudFileClient cloudFileClient;
        private readonly CloudStorageAccount cloudStorageAccount;
        private readonly ILoggerFactory logger;
   
        public TestAzureFile()
        {
            var id = Environment.GetEnvironmentVariable("DEFAULT_ACCOUNT_NAME");
            var key = Environment.GetEnvironmentVariable("DEFAULT_ACCOUNT_KEY");
            var environmentName = Environment.GetEnvironmentVariable("DEFAULT_CLOUD_ENVIRONMENT");

            var csa = new CloudStorageAccount(
                new StorageCredentials(id, key),
                AzureEnvironmentHelper.GetStorageEndpointSuffix(environmentName),
                true
            );
            this.cloudStorageAccount = csa;
            this.cloudFileClient = csa.CreateCloudFileClient();
            this.logger =  TestHelper.CreateLoggerFactory();
        }

        public async Task<List<CloudFileShare>> GetSharesList(){
            List<CloudFileShare> results = new List<CloudFileShare>();
            FileContinuationToken token = null;
            do
            {
                ShareResultSegment resultSegment = await cloudFileClient.ListSharesSegmentedAsync(token);
                results.AddRange(resultSegment.Results);
                token = resultSegment.ContinuationToken;
            }
            while (token != null);
            return results;
        }

        public async Task<List<IListFileItem>> GetRootDirectoryFilesList(string shareName){
            var share = cloudFileClient.GetShareReference(shareName);
            List<IListFileItem> results = new List<IListFileItem>();
            FileContinuationToken token = null;
            do
            {
                FileResultSegment resultSegment = await share.GetRootDirectoryReference().ListFilesAndDirectoriesSegmentedAsync(token);
                results.AddRange(resultSegment.Results);
                token = resultSegment.ContinuationToken;
            }
            while (token != null);
            return results;
        }

         public async Task<int?> CreateShareAsync(string shareName, int? quota)
        {
                var share = cloudFileClient.GetShareReference(shareName);
                share.Properties.Quota = quota;
                await share.CreateAsync();
                await share.FetchAttributesAsync();

                return share.Properties.Quota;
        }
        
    }
}