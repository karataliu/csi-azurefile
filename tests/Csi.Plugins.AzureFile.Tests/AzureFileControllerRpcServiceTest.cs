using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Csi.V0;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Moq;
using Xunit;

namespace Csi.Plugins.AzureFile.Tests
{
    public class AzureFileControllerRpcServiceTest
    {
        [Fact]
        public async Task DeleteVolume()
        {
            var azureFileCsiService = new Mock<IAzureFileCsiService>();
            var service = createService(azureFileCsiService.Object);

            var request = new DeleteVolumeRequest();
            var response = await service.DeleteVolume(request, null);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task DeleteVolumeWithStorageException()
        {
            var azureFileCsiService = new Mock<IAzureFileCsiService>();
            azureFileCsiService
                .Setup(x => x.DeleteVolumeAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
                .Throws<StorageException>();
            var service = createService(azureFileCsiService.Object);

            var request = new DeleteVolumeRequest();
            var ex = await Assert.ThrowsAsync<RpcException>(() => service.DeleteVolume(request, null));
            Assert.Equal(StatusCode.InvalidArgument, ex.Status.StatusCode);
        }

        [Fact]
        public async Task DeleteVolumeWithUnknownException()
        {
            var azureFileCsiService = new Mock<IAzureFileCsiService>();
            azureFileCsiService
                .Setup(x => x.DeleteVolumeAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
                .Throws<Exception>();
            var service = createService(azureFileCsiService.Object);

            var request = new DeleteVolumeRequest();
            var ex = await Assert.ThrowsAsync<RpcException>(() => service.DeleteVolume(request, null));
            Assert.Equal(StatusCode.Internal, ex.Status.StatusCode);
        }

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
                => service.ListVolumes(new ListVolumesRequest(), null));

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

        private AzureFileControllerRpcService createService(IAzureFileCsiService azureFileCsiService)
        {
            var lf = new LoggerFactory();
            return new AzureFileControllerRpcService(
                azureFileCsiService,
                lf.CreateLogger<AzureFileControllerRpcService>());
        }
    }


}
