using Csi.V0;
using Xunit;

namespace Csi.Plugins.AzureFile.Tests
{
    public class SizeConverterTest
    {
        [Fact]
        public void CapacityRangeToQuota()
        {
            var cases = new(CapacityRange, int?)[]
            {
                ( null, null ),
                ( new CapacityRange { RequiredBytes = 2L }, 1 ),
                ( new CapacityRange { RequiredBytes = 2L << 10 }, 1 ),
                ( new CapacityRange { RequiredBytes = 1L << 30 }, 1 ),
                ( new CapacityRange { RequiredBytes = (2L << 30) - 1 }, 2 ),
                ( new CapacityRange { RequiredBytes = (2L << 30)     }, 2 ),
                ( new CapacityRange { RequiredBytes = (2L << 30) + 1 }, 3 ),

                // Might be rejected by server, but fine for converter
                ( new CapacityRange { RequiredBytes = (2L << 50) }, 2 << 20 ),
            };

            foreach (var entry in cases)
            {
                Assert.Equal(entry.Item2, SizeConverter.CapacityRangeToQuota(entry.Item1));
            }
        }

        [Fact]
        public void QuotaToCapacityBytes()
        {
            Assert.Equal(0, SizeConverter.QuotaToCapacityBytes(null));
            Assert.Equal(0, SizeConverter.QuotaToCapacityBytes(0));
            Assert.Equal(1 << 30, SizeConverter.QuotaToCapacityBytes(1));
        }
    }
}
