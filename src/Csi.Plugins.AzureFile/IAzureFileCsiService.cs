using System.Collections.Generic;
using System.Threading.Tasks;
using Csi.V0;

namespace Csi.Plugins.AzureFile
{
    interface IAzureFileCsiService
    {
        Task<Volume> CreateVolumeAsync(string name, IDictionary<string, string> secrets, CapacityRange range);
        Task DeleteVolumeAsync(string volumeId, IDictionary<string, string> secrets);
        (string, SmbShareCredential) GetSmbShare(string volumeId, IDictionary<string, string> secrets);
    }
}
