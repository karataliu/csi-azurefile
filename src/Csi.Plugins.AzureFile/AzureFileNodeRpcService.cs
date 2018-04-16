using System;
using System.Threading.Tasks;
using Csi.V0;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Util.Extensions.Logging.Step;

namespace Csi.Plugins.AzureFile
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

                try
                {
                    (var unc, var cred) = azureFileCsiService.GetSmbShare(id);
                    await smbShareAttacher.AttachAsync(
                        unc,
                        targetPath,
                        cred);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Exception in AttachAsync");
                    throw;
                }

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
                await smbShareAttacher.DetachAsync(targetPath);
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
            var response = new NodeGetCapabilitiesResponse { Capabilities = { } };
            return Task.FromResult(response);
        }
    }
}
