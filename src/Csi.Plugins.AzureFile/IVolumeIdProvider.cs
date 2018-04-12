namespace Csi.Plugins.AzureFile
{
    interface IVolumeIdProvider
    {
        string CreateVolumeId(string storageAccountName, string shareName);
        (string, string) ParseVolumeId(string volumeId);
    }

    class VolumeIdProvider : IVolumeIdProvider
    {
        private const string scheme = "azurefile://";
        public string CreateVolumeId(string storageAccountName, string shareName)
            => $"{scheme}{storageAccountName}/{shareName}";

        public (string, string) ParseVolumeId(string volumeId)
        {
            if (!volumeId.StartsWith(scheme)) throw new System.Exception("Invalid volumeId: " + volumeId);
            var text = volumeId.Substring(scheme.Length);
            var index = text.IndexOf('/');
            if (index < 0) throw new System.Exception("Invalid volumeId: " + volumeId);
            return (text.Substring(0, index), text.Substring(index + 1));
        }
    }
}
