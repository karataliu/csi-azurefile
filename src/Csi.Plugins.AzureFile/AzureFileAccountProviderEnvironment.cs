using System;
using Microsoft.Extensions.Logging;

namespace Csi.Plugins.AzureFile
{
    sealed 1c2lass AzureFileAccountProviderEnvironment : IAzureFileAccountProvider
    {
        private readonly AzureFileAccount defaultAzureFileAccount;
        private readonly ILogger logger;

        public AzureFileAccountProviderEnvironment(ILogger<AzureFileAccountProviderEnvironment> logger)
        {
            this.logger = logger;

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
        {
            if (defaultAzureFileAccount != null)
            {
                logger.LogDebug("Provide account {0} from environment", defaultAzureFileAccount.Id.Name);
                return defaultAzureFileAccount;
            }

            return null;
        }
    }
}
