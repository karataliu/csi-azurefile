using System.Threading.Tasks;

namespace Csi.AzureFile
{
    interface IAzureFileService
    {
        Task<AzureFileShare> CreateShareAsync(string shareName, int? quotaInGib);
        Task DeleteShareAsync(string shareName);
        string GetShareUnc(string shareName);
        SmbShareCredential GetShareCredential();
    }

    sealed class AzureFileShare
    {
        public string Name { get; set; }
        public int? QuotaInGib { get; set; }
    }
}
