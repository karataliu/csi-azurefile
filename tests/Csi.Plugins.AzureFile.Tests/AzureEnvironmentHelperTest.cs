using System;
using Xunit;

namespace Csi.Plugins.AzureFile.Tests
{
    public class AzureEnvironmentHelperTest
    {
        [Fact]
        public void Basic()
        {
            const string azureGlobalCloudSuffix = "core.windows.net";
            const string azureGermanCloudSuffix = "core.cloudapi.de";

            Assert.Equal(azureGlobalCloudSuffix, AzureEnvironmentHelper.GetStorageEndpointSuffix(""));
            Assert.Equal(azureGlobalCloudSuffix, AzureEnvironmentHelper.GetStorageEndpointSuffix("AzureGlobalCloud"));
            Assert.Equal(azureGermanCloudSuffix, AzureEnvironmentHelper.GetStorageEndpointSuffix("AzureGermanCloud"));
            Assert.Equal(azureGermanCloudSuffix, AzureEnvironmentHelper.GetStorageEndpointSuffix("azuregermancloud"));
            Assert.Throws<Exception>(() => AzureEnvironmentHelper.GetStorageEndpointSuffix("a"));
        }
    }
}
