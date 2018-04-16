using Xunit;

namespace Csi.Plugins.AzureFile.Tests
{
    public class SizeConverterTest
    {
        [Fact]
        public void CapacityRangeToQuota()
        {
            var cases = new(long?, int?)[]
            {
                ( null, null ),
                ( -1, null ),
                ( 0, null ),
                ( 2L, 1 ),
                ( 2L << 10 , 1 ),
                ( 1L << 30 , 1 ),
                ( (2L << 30) - 1, 2 ),
                ( (2L << 30)    , 2 ),
                ( (2L << 30) + 1, 3 ),

                // Might be rejected by server, but fine for converter
                ( (2L << 50) , 2 << 20 ),
            };

            foreach (var entry in cases)
            {
                Assert.Equal(entry.Item2, SizeConverter.RequiredBytesToQuota(entry.Item1));
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
