using System.Threading.Tasks;

namespace Csi.Plugins.AzureFile
{
    interface ISmbShareAttacher
    {
        Task AttachAsync(string targetPath, string unc, SmbShareCredential smbShareCredential);
        Task DetachAsync(string targetPath);
    }
}
