using System;

namespace Csi.Plugins.AzureFile
{
    sealed class AzureFileAccountProviderValidator : AzureFileAccountProviderBase
    {
        public override AzureFileAccount Provide(AzureFileAccountProviderContext context)
        {
            var afa = Next.Provide(context);
            if (afa == null) throw new Exception("Cannot provide account");
            return afa;
        }
    }
}
