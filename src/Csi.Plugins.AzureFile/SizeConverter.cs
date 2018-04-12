using Csi.V0;

namespace Csi.Plugins.AzureFile
{
    // Quota in GiB
    static class SizeConverter
    {
        // Ignore limit_bytes
        public static int? CapacityRangeToQuota(CapacityRange range)
        {
            if (range == null || range.RequiredBytes <= 0) return null;

            // round up
            return (int)(((range.RequiredBytes - 1) >> 30) + 1);
        }

        public static long QuotaToCapacityBytes(int? quota)
            => quota == null ? 0 : quota.Value << 30;
    }
}
