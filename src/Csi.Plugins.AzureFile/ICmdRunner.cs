using System.Collections.Generic;
using System.Threading.Tasks;

namespace Csi.Plugins.AzureFile
{
    interface ICmdRunner
    {
        Task RunCmd(CmdEntry cmdEntry);
    }

    sealed class CmdEntry
    {
        public string Command { get; set; }
        public IEnumerable<string> Arguments { get; set; }
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
