using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Util.Extensions.Logging.Step;

namespace Csi.Plugins.AzureFile
{
    sealed class SmbShareAttacherLinux : ISmbShareAttacher
    {
        private readonly ICmdRunner cmdRunner;
        private readonly ILogger logger;

        public SmbShareAttacherLinux(ICmdRunner cmdRunner,ILogger<SmbShareAttacherLinux> logger)
        {
            this.cmdRunner = cmdRunner;
            this.logger = logger;
        }

        public async Task AttachAsync(string unc, string targetPath, SmbShareCredential smbShareCredential)
        {
            using (var _s = logger.StepDebug(nameof(AttachAsync)))
            {
                // Ensure dir exists
                Directory.CreateDirectory(targetPath);
                var cmd = getLinuxConnectCmd(unc, targetPath, smbShareCredential);
                await cmdRunner.RunCmd(cmd);

                _s.Commit();
            }
        }

        public async Task DetachAsync(string targetPath)
        {
            using (var _s = logger.StepDebug(nameof(AttachAsync)))
            {
                var cmd = getLinuxDisconnectCmd(targetPath);
                await cmdRunner.RunCmd(cmd);

                _s.Commit();
            }
        }

        private static CmdEntry getLinuxConnectCmd(
            string unc,
            string targetPath,
            SmbShareCredential smbShareCredential)
        {
            return new CmdEntry
            {
                Command = "mount",
                Arguments = new[]
                {
                    normalizeUnc(unc),
                    targetPath,
                    "-t", "cifs",
                    "-o",
                    $"username={smbShareCredential.Username},password={smbShareCredential.Password}"
                    + ",vers=3.0,dir_mode=0777,file_mode=0777,sec=ntlmssp",
                }
            };
        }

        private static string normalizeUnc(string unc) => unc.Replace('\\', '/');

        public static CmdEntry getLinuxDisconnectCmd(string targetPath)
        {
            return new CmdEntry
            {
                Command = "umount",
                // TODO verify mount from unc?
                Arguments = new[] { targetPath }
            };
        }
    }

}
