using Grpc.Core;

namespace Csi.Plugins.AzureFile.Tests.Scenarios.LocalNode
{

    static class TestHelper
    {
        public static Channel CreateChannel()
            => new Channel("127.0.0.1", 10000, ChannelCredentials.Insecure);
    }
}
