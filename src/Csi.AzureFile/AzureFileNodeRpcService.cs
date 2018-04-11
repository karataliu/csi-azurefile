using System.IO;
using System.Threading.Tasks;
using Csi.V0;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Util.Extensions.Logging.Step;

namespace Csi.AzureFile
{
    sealed class AzureFileNodeRpcService : Node.NodeBase
    {
        private readonly string nodeId;
        private readonly IAzureFileCsiService azureFileCsiService;
        private readonly ISmbShareAttacher smbShareAttacher;
        private readonly ILogger logger;

        public AzureFileNodeRpcService(
            string nodeId,
            IAzureFileCsiService azureFileCsiService,
            ISmbShareAttacher smbAttacher,
            ILogger<AzureFileNodeRpcService> logger)
        {
            this.nodeId = nodeId;
            this.azureFileCsiService = azureFileCsiService;
            this.smbShareAttacher = smbAttacher;
            this.logger = logger;

            logger.LogInformation("Node rpc service loaded, nodeId:{0}", nodeId);
        }

        public override async Task<NodePublishVolumeResponse> NodePublishVolume(
            NodePublishVolumeRequest request, ServerCallContext context)
        {
            var id = request.VolumeId;
            var targetPath = request.TargetPath;
            using (var _s = logger.StepInformation("{0}, id: {1}, targetPath: {2}",
                nameof(NodePublishVolume), id, targetPath))
            {
                logger.LogDebug("{0}: {1}", nameof(NodePublishVolumeRequest), request);

                // Ensure dir exists
                Directory.CreateDirectory(targetPath);

                await smbShareAttacher.AttachAsync(
                    azureFileCsiService.GetSmbShareUnc(id),
                    targetPath,
                    azureFileCsiService.GetShareCredential());
                _s.Commit();
            }

            await Task.CompletedTask;
            return new NodePublishVolumeResponse();
        }

        public override async Task<NodeUnpublishVolumeResponse> NodeUnpublishVolume(
            NodeUnpublishVolumeRequest request, ServerCallContext context)
        {
            var id = request.VolumeId;
            var targetPath = request.TargetPath;
            using (var _s = logger.StepInformation("{0}, id: {1}, targetPath: {2}",
                nameof(NodeUnpublishVolume), id, targetPath))
            {
                await smbShareAttacher.DetachAsync(azureFileCsiService.GetSmbShareUnc(id), targetPath);
                _s.Commit();
            }

            await Task.CompletedTask;
            return new NodeUnpublishVolumeResponse();
        }

        // Actually this is not required for azure file
        // Implement it now for driver-register, see https://github.com/kubernetes-csi/driver-registrar/issues/23
        public override Task<NodeGetIdResponse> NodeGetId(NodeGetIdRequest request, ServerCallContext context)
        {
            var response = new NodeGetIdResponse
            {
                NodeId = this.nodeId,
            };

            return Task.FromResult(response);
        }

        public override Task<NodeGetCapabilitiesResponse> NodeGetCapabilities(
            NodeGetCapabilitiesRequest request,
            ServerCallContext context)
        {
            var response = new NodeGetCapabilitiesResponse
            {
                Capabilities =
                {
                    new NodeServiceCapability {
                        Rpc = new  NodeServiceCapability.Types.RPC{
                            Type = NodeServiceCapability.Types.RPC.Types.Type.Unknown
                        }
                    }
                }
            };

            return Task.FromResult(response);
        }
    }
}
