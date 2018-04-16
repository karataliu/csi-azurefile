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
}
