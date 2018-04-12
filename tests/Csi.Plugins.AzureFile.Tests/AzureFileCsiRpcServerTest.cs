using Xunit;

namespace Csi.Plugins.AzureFile.Tests
{
    public class AzureFileCsiRpcServerTest
    {
        [Fact]
        public void TestGetService()
        {
            var server = new AzureFileCsiRpcServer();
            Assert.NotNull(server.CreateIdentityRpcService());
            Assert.NotNull(server.CreateControllerRpcService());
            Assert.NotNull(server.CreateNodeRpcService());
        }
    }
}
