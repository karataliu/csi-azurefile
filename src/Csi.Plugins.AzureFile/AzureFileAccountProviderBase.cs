using System;

namespace Csi.Plugins.AzureFile
{
    abstract class AzureFileAccountProviderBase : IAzureFileAccountProvider
    {
        public IAzureFileAccountProvider Next { get; set; }

        public abstract AzureFileAccount Provide(AzureFileAccountProviderContext context);
    }
}
