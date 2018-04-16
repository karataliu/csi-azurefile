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
            var defaultAccountName = Environment.GetEnvironmentVariable("DEFAULT_ACCOUNT_NAME");
            var defaultAccountKey = Environment.GetEnvironmentVariable("DEFAULT_ACCOUNT_KEY");
            if (!string.IsNullOrEmpty(defaultAccountName) && !string.IsNullOrEmpty(defaultAccountKey))
            {
                defaultAzureFileAccount = new AzureFileAccount
                {
                    Id = new AzureFileAccountId
                    {
                        Name = defaultAccountName,
                    },
                    Key = defaultAccountKey,
                };

                var defaultEnvironmentName = Environment.GetEnvironmentVariable("DEFAULT_CLOUD_ENVIRONMENT");
                if (!string.IsNullOrEmpty(defaultEnvironmentName))
                    defaultAzureFileAccount.Id.EnvironmentName = defaultEnvironmentName;
            }
        }

        public AzureFileAccount Provide(AzureFileAccountProviderContext context)
            => defaultAzureFileAccount;
    }
}
