using System;

namespace Csi.Plugins.AzureFile
{
    interface IAzureFileAccountProvider
    {
        AzureFileAccount Provide(AzureFileAccountProviderContext context);
    }

    sealed class AzureFileAccountProviderContext
    {
    }

    sealed class DefaultAzureFileAccountProvider : IAzureFileAccountProvider
    {
        private readonly AzureFileAccount defaultAzureFileAccount;
        public DefaultAzureFileAccountProvider()
        {
            var defaultEnvironmentName = Environment.GetEnvironmentVariable("DEFAULT_CLOUD_ENVIRONMENT");
            var defaultAccountName = Environment.GetEnvironmentVariable("DEFAULT_ACCOUNT_NAME");
            var defaultAccountKey = Environment.GetEnvironmentVariable("DEFAULT_ACCOUNT_KEY");
            if (!string.IsNullOrEmpty(defaultAccountName) && !string.IsNullOrEmpty(defaultAccountKey))
                defaultAzureFileAccount = new AzureFileAccount
                {
                    Id = new AzureFileAccountId
                    {
                        EnvironmentName = defaultEnvironmentName,
                        Name = defaultAccountName,
                    },
                    Key = defaultAccountKey,
                };
        }

        public AzureFileAccount Provide(AzureFileAccountProviderContext context)
            => defaultAzureFileAccount;
    }
}
