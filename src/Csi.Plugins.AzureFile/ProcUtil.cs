using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Util.Extensions.Logging.Step;

namespace Csi.Plugins.AzureFile
{
    sealed class CmdEntry
    {
        public string Command { get; set; }
        public IEnumerable<string> Arguments { get; set; }
    }

    static class ProcUtil
    {
        public static bool ShowStdout { get; set; } = true;

        public static async Task RunCmd(CmdEntry cmdEntry, ILogger logger)
        {
            var cmd = cmdEntry.Command;
            var arguments = cmdEntry.Arguments;
            var argumentsStr = string.Join(" ", arguments.Select(a => $"\"{a}\""));
            var info = new ProcessStartInfo
            {
                FileName = cmd,
                Arguments = argumentsStr,
                UseShellExecute = false,
                RedirectStandardOutput = !ShowStdout,
            };

            int exitCode = 0;
            using (var _s = logger.StepDebug("Run command: {0}", cmd))
            {
                // logger.LogDebug("Cmd: '{0} {1}", cmd, argumentsStr);
                await Task.Run(() =>
                {
                    using (var process = Process.Start(info))
                    {
                        if (!ShowStdout)
                        {
                            process.StandardOutput.ReadToEnd();
                        }
                        process.WaitForExit();
                        exitCode = process.ExitCode;
                    }
                });

                logger.LogDebug("exit code: {0}", exitCode);
                if (exitCode != 0)
                {
                    throw new System.Exception("Cmd failed");
                }

                _s.Commit();
            }
        }
    }
}
