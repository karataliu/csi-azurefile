using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Csi.Plugins.AzureFile
{
    sealed class SmbShareAttacherWindows : ISmbShareAttacher
    {
        private readonly ILogger logger;
        private ICmdRunner cmdRunner;

        public SmbShareAttacherWindows(ICmdRunner cmdRunner, ILogger<SmbShareAttacherWindows> logger)
        {
            this.cmdRunner = cmdRunner;
            this.logger = logger;
        }

        public Task AttachAsync(string targetPath, string unc, SmbShareCredential smbShareCredential)
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

    static class CmdRunnerExtensions
    {
        private const string powerShellBin = "powershell";
        private const string powerShellCommand = "-Command";

        public static Task RunPowerShell(this ICmdRunner cmdRunner, IEnumerable<string> scriptLines)
        {
            return cmdRunner.RunCmd(new CmdEntry
            {
                Command = powerShellBin,
                Arguments = new[] { powerShellCommand, string.Join(';', scriptLines) },
            });
        }
    }
}
