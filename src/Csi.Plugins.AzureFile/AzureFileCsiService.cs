using System.Collections.Generic;
using System.Threading.Tasks;
using Csi.V0;
using Microsoft.Extensions.Logging;

namespace Csi.Plugins.AzureFile
{
    sealed class AzureFileCsiService : IAzureFileCsiService
    {
        private readonly IAzureFileAccountProvider azureFileAccountProvider;
        private readonly IAzureFileServiceFactory azureFileServiceFactory;
        private readonly IVolumeIdProvider volumeIdProvider;
        private readonly AzureFileAccountIdValidator validator;
        private readonly ILogger logger;

        public AzureFileCsiService(
            IAzureFileAccountProvider azureFileAccountProvider,
            IAzureFileServiceFactory azureFileServiceFactory,
            IVolumeIdProvider volumeIdProvider,
            ILogger<AzureFileCsiService> logger)
        {
            this.azureFileAccountProvider = azureFileAccountProvider;
            this.azureFileServiceFactory = azureFileServiceFactory;
            this.volumeIdProvider = volumeIdProvider;
            this.logger = logger;

            this.validator = new AzureFileAccountIdValidator(logger);
        }

        public async Task<Volume> CreateVolumeAsync(
            string name,
            IDictionary<string, string> secrets,
            CapacityRange range)
        {
            var azureFileAccount = azureFileAccountProvider.Provide(new AzureFileAccountProviderContext(secrets));
            var azureFileService = azureFileServiceFactory.Create(azureFileAccount);
            var shareName = name;
            // Ignore limit_bytes
            var share = await azureFileService.CreateShareAsync(shareName,
                SizeConverter.RequiredBytesToQuota(range?.RequiredBytes));

            var shareId = new AzureFileShareId
            {
                ShareName = share.Name,
                AccountId = azureFileAccount.Id,
            };

            return new Volume
            {
                Id = volumeIdProvider.CreateVolumeId(shareId),
                CapacityBytes = SizeConverter.QuotaToCapacityBytes(share.QuotaInGib),
            };
        }

        public async Task DeleteVolumeAsync(string volumeId, IDictionary<string, string> secrets)
        {
            var shareId = volumeIdProvider.ParseVolumeId(volumeId);
            var azureFileAccount = azureFileAccountProvider.Provide(new AzureFileAccountProviderContext(secrets));

            validator.Validate(shareId.AccountId, azureFileAccount.Id);

            var azureFileService = azureFileServiceFactory.Create(azureFileAccount);
            await azureFileService.DeleteShareAsync(shareId.ShareName);
        }

        public (string, SmbShareCredential) GetSmbShare(string volumeId, IDictionary<string, string> secrets)
        {
            var shareId = volumeIdProvider.ParseVolumeId(volumeId);
            var azureFileAccount = azureFileAccountProvider.Provide(new AzureFileAccountProviderContext(secrets));

            validator.Validate(shareId.AccountId, azureFileAccount.Id);

            var azureFileService = azureFileServiceFactory.Create(azureFileAccount);
            return (azureFileService.GetShareUnc(shareId.ShareName), azureFileService.GetShareCredential());
        }
    }

    sealed class AzureFileAccountIdValidator
    {
        private readonly ILogger logger;

        public AzureFileAccountIdValidator(ILogger logger) => this.logger = logger;

        public void Validate(AzureFileAccountId expected, AzureFileAccountId provided)
        {
            var expectedStr = toIdString(expected);
            var providedStr = toIdString(provided);
            logger.LogDebug("Expected: {0}, provided: {1}", expectedStr, providedStr);
            if (expectedStr != providedStr) throw new System.Exception("Provided account does not match expected");
        }

        private string toIdString(AzureFileAccountId afai) => $"{afai.Name}@{afai.EnvironmentName}".ToLower();
    }
}
