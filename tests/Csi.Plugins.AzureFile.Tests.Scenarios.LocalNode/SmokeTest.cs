using System.Threading.Tasks;
using Csi.V0;
using Xunit;

namespace Csi.Plugins.AzureFile.Tests.Scenarios.LocalNode
{
    public class SmokeTest
    {
        [Fact]
        public async Task TestGetPluginInfo()
        {
            var ic = new Identity.IdentityClient(TestHelper.CreateChannel());
            var response = await ic.GetPluginInfoAsync(new GetPluginInfoRequest(), null);
            Assert.Equal("csi-azurefile", response.Name);
            Assert.Equal("1.0.0-alpha1", response.VendorVersion);
        }
    }
}
