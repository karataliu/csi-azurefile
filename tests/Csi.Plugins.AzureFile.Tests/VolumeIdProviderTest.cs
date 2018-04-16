using System;
using Xunit;

namespace Csi.Plugins.AzureFile.Tests
{
    public class VolumeIdProviderTest
    {
        [Fact]
        public void CreateVolumeId()
        {
            var p = new VolumeIdProvider();
            var shareId = new AzureFileShareId
            {
                AccountId = new AzureFileAccountId
                {
                    Name = "a1",
                    EnvironmentName = "E1",
                },
                ShareName = "b2",
            };
            Assert.Equal("azurefile://a1.e1/b2", p.CreateVolumeId(shareId));
        }

        [Fact]
        public void ParseVolmeId()
        {
            var p = new VolumeIdProvider();
            var shareId = p.ParseVolumeId("azurefile://c1.Env2/d2");
            Assert.Equal("env2", shareId.AccountId.EnvironmentName);
            Assert.Equal("c1", shareId.AccountId.Name);
            Assert.Equal("d2", shareId.ShareName);
        }

        [Fact]
        public void ParseInvalideVolmeId()
        {
            var p = new VolumeIdProvider();
            Assert.Throws<Exception>(() => p.ParseVolumeId(""));
            Assert.Throws<Exception>(() => p.ParseVolumeId("azurefile://c1.Env2"));
            Assert.Throws<Exception>(() => p.ParseVolumeId("azurefile://c1/a"));
        }
    }
}
