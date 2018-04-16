using System;
using Microsoft.Extensions.Logging;

namespace Csi.Plugins.AzureFile
{
    sealed class AzureFileAccountProviderDynamicSecret : AzureFileAccountProviderBase
    {
        private const string nameCloudEnv = "cloudEnv";
        private const string nameAccountName = "accountName";
        private const string nameAccountKey = "accountKey";

        private readonly ILogger logger;

        public AzureFileAccountProviderDynamicSecret(ILogger<AzureFileAccountProviderDynamicSecret> logger)
        {
            this.logger = logger;
        }

        public override AzureFileAccount Provide(AzureFileAccountProviderContext context)
        {
            if (context.secrets.TryGetValue(nameAccountName, out var name))
            {
                if (!context.secrets.TryGetValue(nameAccountKey, out var key)) throw new Exception("No key provided");
                var afa = new AzureFileAccount
                {
                    Id = new AzureFileAccountId
                    {
                        Name = name,
                    },
                    Key = key,
                };

                if (context.secrets.TryGetValue(nameCloudEnv, out var env) && !string.IsNullOrEmpty(env))
                {
                    afa.Id.EnvironmentName = env;
                }

                logger.LogDebug("Provide account {0} from secret", name);

                return afa;
            }

            return Next.Provide(context);
        }
    }
}
