using System;
using System.Threading.Tasks;

namespace Csi.Plugins.AzureFile
{
    class SmbShareAttacherWindows : ISmbShareAttacher
    {
        public Task AttachAsync(string unc, string targetPath, SmbShareCredential smbShareCredential)
        {
            throw new NotImplementedException();
        }

        public Task DetachAsync(string unc, string targetPath)
        {
            throw new NotImplementedException();
        }
    }
}
