using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Csi.Plugins.AzureFile
{
    class SmbShareAttacherWindows : ISmbShareAttacher
    {
        private readonly ILogger logger;
        private ICmdRunner cmdRunner;

        public SmbShareAttacherWindows(ICmdRunner cmdRunner, ILogger<SmbShareAttacherWindows> logger)
        {
            this.cmdRunner = cmdRunner;
            this.logger = logger;
        }

        public Task AttachAsync(string unc, string targetPath, SmbShareCredential smbShareCredential)
        {
            var script = new[]
            {
                $"$acctKey = ConvertTo-SecureString -String \"{smbShareCredential.Password}\" -AsPlainText -Force",
                $"$credential = New-Object System.Management.Automation.PSCredential -ArgumentList \"Azure\\{smbShareCredential.Username}\", $acctKey",
                $"New-SmbGlobalMapping -Credential $credential -RemotePath {unc}",
                $"New-Item -ItemType SymbolicLink -Path {targetPath} -Value {unc}",
            };
            return cmdRunner.RunPowerShell(script);
        }

        public Task DetachAsync(string targetPath)
        {
            var script = new[]
            {
                $"$dir=Get-Item -Path {targetPath}",
                $"$dir.Delete()",
                //$"Remove-SmbGlobalMapping ",
            };
            return cmdRunner.RunPowerShell(script);
        }
    }
}
