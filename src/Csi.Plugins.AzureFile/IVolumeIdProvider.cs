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

    class VolumeIdProvider : IVolumeIdProvider
    {
        private const string scheme = "azurefile://";
        public string CreateVolumeId(AzureFileShareId shareId)
            => $"{scheme}{shareId.AccountId.Name}.{shareId.AccountId.EnvironmentName}/{shareId.ShareName}".ToLower();

        public AzureFileShareId ParseVolumeId(string volumeId)
        {
            if (!volumeId.StartsWith(scheme)) throw new System.Exception("Invalid volumeId: " + volumeId);
            var text = volumeId.Substring(scheme.Length).ToLower();
            var index = text.IndexOf('/');
            if (index < 0) throw new System.Exception("Invalid volumeId: " + volumeId);
            
            var shareName = text.Substring(index + 1);
            var nameAndEnv = text.Substring(0, index);
            var indexDot = nameAndEnv.IndexOf(".");
            if (indexDot < 0) throw new System.Exception("Invalid volumeId: " + volumeId);
            var name = nameAndEnv.Substring(0, indexDot);
            var env = nameAndEnv.Substring(indexDot + 1);
            return new AzureFileShareId {
                AccountId = new AzureFileAccountId
                {
                    EnvironmentName = env,
                    Name = name,
                },
                ShareName = shareName,
            };
        }
    }
}
