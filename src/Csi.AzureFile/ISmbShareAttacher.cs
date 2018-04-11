using System.Threading.Tasks;

namespace Csi.AzureFile
{
    interface ISmbShareAttacher
    {
        Task AttachAsync(string unc, string targetPath, SmbShareCredential smbShareCredential);
        Task DetachAsync(string unc, string targetPath);
    }
}
