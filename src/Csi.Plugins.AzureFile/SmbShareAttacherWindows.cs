using System.Threading.Tasks;
using Csi.Helpers.Azure;
using Microsoft.Extensions.Logging;

namespace Csi.Plugins.AzureFile
{
    sealed class SmbShareAttacherWindows : ISmbShareAttacher
    {
        private readonly ILogger logger;
        private IExternalRunner cmdRunner;

        public SmbShareAttacherWindows(IExternalRunner cmdRunner, ILogger<SmbShareAttacherWindows> logger)
        {
            this.cmdRunner = cmdRunner;
            this.logger = logger;
        }

        public Task AttachAsync(string targetPath, string unc, SmbShareCredential smbShareCredential)
        {
            var script = $@"
$acctKey = ConvertTo-SecureString -String ""{smbShareCredential.Password}"" -AsPlainText -Force;
$credential = New-Object System.Management.Automation.PSCredential -ArgumentList ""Azure\{smbShareCredential.Username}"", $acctKey;
New-SmbGlobalMapping -Credential $credential -RemotePath {unc};
New-Item -ItemType SymbolicLink -Path {targetPath} -Value {unc};
";

            return cmdRunner.RunPowershell(script);
        }

        public Task DetachAsync(string targetPath)
        {
            var script = $@"
$dir=Get-Item -Path {targetPath};
$dir.Delete();
";
            //$"Remove-SmbGlobalMapping ",
            return cmdRunner.RunPowershell(script);
        }
    }
}
