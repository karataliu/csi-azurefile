using System;
using Xunit;

namespace Csi.AzureFile.Tests
{
    public class VolumeIdProviderTest
    {
        [Fact]
        public void CreateVolumeId()
        {
            var p = new VolumeIdProvider();
            Assert.Equal("azurefile://a1/b2", p.CreateVolumeId("a1", "b2"));
        }

        [Fact]
        public void ParseVolmeId()
        {
            var p = new VolumeIdProvider();
            (var account, var share) = p.ParseVolumeId("azurefile://c1/d2");
            Assert.Equal("c1", account);
            Assert.Equal("d2", share);
        }

        [Fact]
        public void ParseInvalideVolmeId()
        {
            var p = new VolumeIdProvider();
            Assert.Throws<Exception>(() => p.ParseVolumeId(""));
        }
    }
}
