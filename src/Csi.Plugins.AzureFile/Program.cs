using System.Threading;
using Csi.V0.Server;

namespace Csi.Plugins.AzureFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var csiRpcServer = new CsiRpcServer(new AzureFileCsiRpcServiceFactory());
            csiRpcServer.ConfigFromEnvironment();
            csiRpcServer.Start();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
