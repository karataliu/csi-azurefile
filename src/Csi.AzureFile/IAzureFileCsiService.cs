using System.Threading.Tasks;
using Csi.V0;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Logging;

namespace Csi.AzureFile
{
    interface IAzureFileCsiService
    {
        Task<Volume> CreateVolumeAsync(string name,
            MapField<string, string> parameters,
            CapacityRange range1);
        Task DeleteVolumeAsync(string volumeId);
        string GetSmbShareUnc(string volumeId);
        SmbShareCredential GetShareCredential();
    }

    class AzureFileCsiService : IAzureFileCsiService
    {
        private readonly IAzureFileServiceFactory azureFileServiceFactory;
        private readonly ILogger logger;

        public AzureFileCsiService(IAzureFileServiceFactory azureFileServiceFactory, ILogger<AzureFileCsiService> logger)
        {
            this.azureFileServiceFactory = azureFileServiceFactory;
            this.logger = logger;
        }

        public async Task<Volume> CreateVolumeAsync(
            string name,
            MapField<string, string> parameters,
            CapacityRange range1)
        {
            int? getCapacity(CapacityRange range)
            {
                if (range == null || range.LimitBytes <= 0) return null;
                return (int)(range.LimitBytes >> 30);
            }

            var shareName = name;
            if (parameters.TryGetValue("shareName", out var sn)) shareName = sn;
            var capacity = getCapacity(range1);

            var azureFileService = azureFileServiceFactory.Create();
            var share = await azureFileService.CreateShareAsync(shareName, capacity);
            var quota = share.Properties.Quota;

            return new Volume
            {
                Id = share.Name,
                CapacityBytes = quota == null ? 0 : quota.Value << 30,
            };
        }

        public async Task DeleteVolumeAsync(string volumeId)
        {
            var azureFileService = azureFileServiceFactory.Create();
            var shareName = volumeId;
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
            var shareName = volumeId;
            return azureFileService.GetShareUnc(shareName);
        }
    }
}
