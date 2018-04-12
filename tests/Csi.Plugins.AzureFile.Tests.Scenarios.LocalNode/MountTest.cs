using System;
using System.IO;
using System.Threading.Tasks;
using Csi.V0;
using Xunit;

namespace Csi.Plugins.AzureFile.Tests.Scenarios.LocalNode
{
    public class VolumeAttachTest
    {
        private readonly NamingProvider namingProvider = new NamingProvider();

        [Fact]
        public async Task Attach()
        {
            var channel = TestHelper.CreateChannel();
            var controller = new Controller.ControllerClient(channel);
            var request = new CreateVolumeRequest
            {
                Name = namingProvider.VolumeName(),
            };
            var response = await controller.CreateVolumeAsync(request, null);
            Assert.NotNull(response.Volume);
            Assert.NotNull(response.Volume.Id);
            var volId = response.Volume.Id;

            var node = new Node.NodeClient(TestHelper.CreateChannel());
            var tmppath = Path.Combine(Path.GetTempPath(), namingProvider.DirName());
            try
            {
                var npr = new NodePublishVolumeRequest
                {
                    VolumeId = volId,
                    TargetPath = tmppath,
                };
                await node.NodePublishVolumeAsync(npr, null);
                var tmpfile = Path.Combine(tmppath, namingProvider.FileName());
                var content = DateTimeOffset.UtcNow.ToString();
                await File.AppendAllTextAsync(tmpfile, content);

                // TODO valid content through storage API, and enable DeleteVolumeAsync
            }
            finally
            {
                await node.NodeUnpublishVolumeAsync(new NodeUnpublishVolumeRequest
                {
                    VolumeId = volId,
                    TargetPath = tmppath,
                }, null);

                /*
                await controller.DeleteVolumeAsync(new DeleteVolumeRequest
                {
                    VolumeId = volId,
                });
                */
            }
        }
    }

    class NamingProvider
    {
        public string DirName() => randomStr();
        public string FileName() => randomStr() + ".txt";
        public string FileContent() => randomStr() + DateTimeOffset.UtcNow.ToString();
        public string VolumeName() => "localnode-test" + randomStr();
        private string randomStr() => Guid.NewGuid().ToString();
    }
}
