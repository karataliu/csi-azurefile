using System.Threading.Tasks;
using Csi.V0;
using Microsoft.Extensions.Logging;

namespace Csi.AzureFile
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
        private readonly IAzureFileServiceFactory azureFileServiceFactory;
        private readonly IVolumeIdProvider volumeIdProvider;
        private readonly ILogger logger;

        public AzureFileCsiService(IAzureFileServiceFactory azureFileServiceFactory, IVolumeIdProvider volumeIdProvider,
            ILogger logger)
        {
            this.azureFileServiceFactory = azureFileServiceFactory;
            this.volumeIdProvider = volumeIdProvider;
            this.logger = logger;
        }

        public async Task<Volume> CreateVolumeAsync(
            string name,
            CapacityRange range)
        {
            var azureFileService = azureFileServiceFactory.Create();
            var shareName = name;
            var share = await azureFileService.CreateShareAsync(shareName,
                SizeConverter.CapacityRangeToQuota(range));

            return new Volume
            {
                Id = volumeIdProvider.CreateVolumeId("0", share.Name),
                CapacityBytes = SizeConverter.QuotaToCapacityBytes(share.QuotaInGib),
            };
        }

        public async Task DeleteVolumeAsync(string volumeId)
        {
            var azureFileService = azureFileServiceFactory.Create();
            (var account, var shareName) = volumeIdProvider.ParseVolumeId(volumeId);
            await azureFileService.DeleteShareAsync(shareName);
        }

        public SmbShareCredential GetShareCredential()
        {
            var azureFileService = azureFileServiceFactory.Create();
            return azureFileService.GetShareCredential();
        }

        public string GetSmbShareUnc(string volumeId)
        {
            var azureFileService = azureFileServiceFactory.Create();
            (var account, var shareName) = volumeIdProvider.ParseVolumeId(volumeId);
            return azureFileService.GetShareUnc(shareName);
        }
    }
}
