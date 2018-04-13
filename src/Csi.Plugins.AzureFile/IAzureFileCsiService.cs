using System.Threading.Tasks;
using Csi.V0;
using Microsoft.Extensions.Logging;

namespace Csi.Plugins.AzureFile
{
    interface IAzureFileCsiService
    {
        Task<Volume> CreateVolumeAsync(string name,
            CapacityRange range);
        Task DeleteVolumeAsync(string volumeId);
        string GetSmbShareUnc(string volumeId);
        SmbShareCredential GetShareCredential();
    }

    class AzureFileCsiService : IAzureFileCsiService
    {
        private readonly IAzureFileAccountProvider azureFileAccountProvider;
        private readonly IAzureFileServiceFactory azureFileServiceFactory;
        private readonly IVolumeIdProvider volumeIdProvider;
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
        }

        public async Task<Volume> CreateVolumeAsync(
            string name,
            CapacityRange range)
        {
            var azureFileAccount = azureFileAccountProvider.Provide(new AzureFileAccountProviderContext());
            var azureFileService = azureFileServiceFactory.Create(azureFileAccount);
            var shareName = name;
            // Ignore limit_bytes
            var share = await azureFileService.CreateShareAsync(shareName,
                SizeConverter.RequiredBytesToQuota(range.RequiredBytes));

            return new Volume
            {
                Id = volumeIdProvider.CreateVolumeId("0", share.Name),
                CapacityBytes = SizeConverter.QuotaToCapacityBytes(share.QuotaInGib),
            };
        }

        public async Task DeleteVolumeAsync(string volumeId)
        {
            var azureFileAccount = azureFileAccountProvider.Provide(new AzureFileAccountProviderContext());
            var azureFileService = azureFileServiceFactory.Create(azureFileAccount);
            (var account, var shareName) = volumeIdProvider.ParseVolumeId(volumeId);
            await azureFileService.DeleteShareAsync(shareName);
        }

        public SmbShareCredential GetShareCredential()
        {
            var azureFileAccount = azureFileAccountProvider.Provide(new AzureFileAccountProviderContext());
            var azureFileService = azureFileServiceFactory.Create(azureFileAccount);
            return azureFileService.GetShareCredential();
        }

        public string GetSmbShareUnc(string volumeId)
        {
            var azureFileAccount = azureFileAccountProvider.Provide(new AzureFileAccountProviderContext());
            var azureFileService = azureFileServiceFactory.Create(azureFileAccount);
            (var account, var shareName) = volumeIdProvider.ParseVolumeId(volumeId);
            return azureFileService.GetShareUnc(shareName);
        }
    }
}
