namespace Csi.Plugins.AzureFile
{
    interface IVolumeIdProvider
    {
        string CreateVolumeId(AzureFileShareId shareId);
        AzureFileShareId ParseVolumeId(string volumeId);
    }

    sealed class AzureFileShareId
    {
        public AzureFileAccountId AccountId { get; set; }
        public string ShareName { get; set; }
    }
}
