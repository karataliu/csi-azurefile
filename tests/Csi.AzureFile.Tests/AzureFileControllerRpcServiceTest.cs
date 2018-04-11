using System.Threading.Tasks;
using Csi.V0;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Csi.AzureFile.Tests
{
    public class AzureFileControllerRpcServiceTest
    {
        [Fact]
        public async Task ControllerGetCapabilities()
        {
            var lf = new LoggerFactory();
            var controllerRpcService = new AzureFileControllerRpcService(
                null,
                lf.CreateLogger<AzureFileControllerRpcService>());

            var request = new ControllerGetCapabilitiesRequest();
            var response = await controllerRpcService.ControllerGetCapabilities(request, null);
            Assert.Single(response.Capabilities);
            Assert.Equal(ControllerServiceCapability.Types.RPC.Types.Type.CreateDeleteVolume,
                response.Capabilities[0].Rpc.Type);
        }

        [Fact]
        public async Task ControllerPublishVolumeShouldThrowRpcUnimplementedException()
        {
            var lf = new LoggerFactory();
            var controllerRpcService = new AzureFileControllerRpcService(
                null,
                lf.CreateLogger<AzureFileControllerRpcService>());

            var request = new ControllerPublishVolumeRequest();
            var ex = await Assert.ThrowsAsync<RpcException>(()
                => controllerRpcService.ControllerPublishVolume(request, null));

            Assert.Equal(StatusCode.Unimplemented, ex.Status.StatusCode);
            Assert.Equal(1, 2);
        }
    }
}
