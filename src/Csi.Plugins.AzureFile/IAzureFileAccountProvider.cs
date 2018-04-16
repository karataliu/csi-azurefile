using System.Collections.Generic;

namespace Csi.Plugins.AzureFile
{
    interface IAzureFileAccountProvider
    {
        AzureFileAccount Provide(AzureFileAccountProviderContext context);
    }

    sealed class AzureFileAccountProviderContext
    {
        public AzureFileAccountProviderContext(IDictionary<string, string> secrets)
        {
            this.secrets = secrets;
        }

        public IDictionary<string, string> secrets { get; set; }
    }
}
