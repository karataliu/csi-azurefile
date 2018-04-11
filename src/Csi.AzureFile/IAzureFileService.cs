using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.File;

namespace Csi.AzureFile
{
    interface IAzureFileService
    {
        Task<CloudFileShare> CreateShareAsync(string shareName, int? capacity);
        Task DeleteShareAsync(string shareName);
        string GetShareUnc(string shareName);
        SmbShareCredential GetShareCredential();
    }
}
