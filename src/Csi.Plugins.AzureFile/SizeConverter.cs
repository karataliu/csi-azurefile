namespace Csi.Plugins.AzureFile
{
    // Quota in GiB
    static class SizeConverter
    {
        public static int? RequiredBytesToQuota(long? requiredBytes)
        {
            if (requiredBytes == null || requiredBytes <= 0) return null;
            // round up
            return (int)(((requiredBytes.Value - 1) >> 30) + 1);
        }

        public static long QuotaToCapacityBytes(int? quota)
            => quota == null ? 0 : quota.Value << 30;
    }
}
