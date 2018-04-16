using System;
using Xunit;

namespace Csi.Plugins.AzureFile.Tests
{
    public class AzureEnvironmentHelperTest
    {
        [Fact]
        public void Basic()
        {
            var azureGlobalCloudSuffix = AzureEnvironmentHelper.GetStorageEndpointSuffix("AzureGlobalCloud");
            Assert.False( string.IsNullOrEmpty(azureGlobalCloudSuffix));
            Assert.Equal(azureGlobalCloudSuffix, AzureEnvironmentHelper.GetStorageEndpointSuffix(""));
            Assert.Equal(azureGlobalCloudSuffix, AzureEnvironmentHelper.GetStorageEndpointSuffix("AzureGlobalCloud"));

            var azureGermanCloudSuffix = AzureEnvironmentHelper.GetStorageEndpointSuffix("AzureGermanCloud");
            Assert.False(string.IsNullOrEmpty(azureGermanCloudSuffix));
            Assert.NotEqual(azureGlobalCloudSuffix, azureGermanCloudSuffix);
            Assert.Equal(azureGermanCloudSuffix, AzureEnvironmentHelper.GetStorageEndpointSuffix("azuregermancloud"));

            Assert.Throws<Exception>(() => AzureEnvironmentHelper.GetStorageEndpointSuffix("a"));
        }
    }
}
