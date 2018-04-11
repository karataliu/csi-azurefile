using System;
using System.Threading.Tasks;
using Csi.V0;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Util.Extensions.Logging.Step;

namespace Csi.AzureFile
{
    sealed class AzureFileControllerRpcService : Controller.ControllerBase
    {
        private readonly IAzureFileCsiService azureFileCsiService;
        private readonly ILogger logger;

        public AzureFileControllerRpcService(
            IAzureFileCsiService azureFileCsiService,
            ILogger<AzureFileControllerRpcService> logger)
        {
            this.azureFileCsiService = azureFileCsiService;
            this.logger = logger;
        }

        public override async Task<CreateVolumeResponse> CreateVolume(
            CreateVolumeRequest request,
            ServerCallContext context)
        {
            CreateVolumeResponse response = new CreateVolumeResponse();

            logger.LogDebug("{0}:{1}", nameof(CreateVolumeRequest), request);
            if (string.IsNullOrEmpty(request.Name))
            {
                logger.LogDebug("Validation fail");
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Name cannot be empty"));
            }

            using (var _s = logger.StepInformation("{0}, name: {1}", nameof(CreateVolume), request.Name))
            {
                try
                {
                    response.Volume = await azureFileCsiService.CreateVolumeAsync(
                        request.Name,
                        request.CapacityRange);
                }
                catch (StorageException ex)
                {
                    logger.LogWarning(ex, "Error from storage service");
                    _s.Commit();
                    throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Exception in CreateVolume");
                    throw new RpcException(new Status(StatusCode.Internal, ex.Message));
                }

                _s.Commit();
            }

            return response;
        }

        public override async Task<DeleteVolumeResponse> DeleteVolume(
            DeleteVolumeRequest request,
            ServerCallContext context)
        {
            DeleteVolumeResponse response = new DeleteVolumeResponse();
            var id = request.VolumeId;
            using (var _s = logger.StepInformation("{0}, id: {1}", nameof(DeleteVolume), id))
            {
                await azureFileCsiService.DeleteVolumeAsync(id);
                _s.Commit();
            }

            return response;
        }

        public override Task<ControllerGetCapabilitiesResponse> ControllerGetCapabilities(
            ControllerGetCapabilitiesRequest request, ServerCallContext context)
        {
            logger.LogInformation(nameof(ControllerGetCapabilities));

            var rp = new ControllerGetCapabilitiesResponse
            {
                Capabilities =
                {
                    new ControllerServiceCapability
                    {
                        Rpc = new ControllerServiceCapability.Types.RPC
                        {
                            Type = ControllerServiceCapability.Types.RPC.Types.Type.CreateDeleteVolume
                        }
                    }
                }
            };

            return Task.FromResult(rp);
        }
    }
}
