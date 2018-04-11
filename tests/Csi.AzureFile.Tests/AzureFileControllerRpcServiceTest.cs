using System.Threading.Tasks;
using Csi.V0;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Csi.AzureFile.Tests
{
    public class AzureFileControllerRpcServiceTest
    {
        [Fact]
        public async Task ControllerGetCapabilities()
        {
            var service = createService();

            var response = await service.ControllerGetCapabilities(new ControllerGetCapabilitiesRequest(), null);
            Assert.Single(response.Capabilities);
            Assert.Equal(ControllerServiceCapability.Types.RPC.Types.Type.CreateDeleteVolume,
                response.Capabilities[0].Rpc.Type);
        }

        [Fact]
        public async Task UnsupportedApiShouldThrowRpcUnimplementedException()
        {
            var service = createService();

            await AssertRpc.ThrowsRpcUnimplementedException(()
                => service.ControllerPublishVolume(new ControllerPublishVolumeRequest(), null));
            await AssertRpc.ThrowsRpcUnimplementedException(()
                => service.ControllerUnpublishVolume(new ControllerUnpublishVolumeRequest(), null));

            await AssertRpc.ThrowsRpcUnimplementedException(()
                => service.ValidateVolumeCapabilities(new ValidateVolumeCapabilitiesRequest(), null));

            await AssertRpc.ThrowsRpcUnimplementedException(()
                => service.ListVolumes(new ListVolumesRequest (), null));

            await AssertRpc.ThrowsRpcUnimplementedException(()
                => service.GetCapacity(new GetCapacityRequest(), null));

            await AssertRpc.ThrowsRpcUnimplementedException(()
                => service.GetCapacity(new GetCapacityRequest(), null));
        }

        private AzureFileControllerRpcService createService()
        {
            var lf = new LoggerFactory();
            return new AzureFileControllerRpcService(
                null,
                lf.CreateLogger<AzureFileControllerRpcService>());
        }
    }

   
}
