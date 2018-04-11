using Xunit;

namespace Csi.AzureFile.Tests
{
    public class AzureFileCsiRpcServerTest
    {
        [Fact]
        public void TestGetService()
        {
            var server = new AzureFileCsiRpcServer();
            server.CreateIdentityRpcService();
            server.CreateControllerRpcService();
            server.CreateNodeRpcService();
        }
    }
}
