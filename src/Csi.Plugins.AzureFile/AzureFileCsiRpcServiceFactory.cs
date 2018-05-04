using System;
using Csi.Helpers.Azure;
using Csi.V0;
using Csi.V0.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Csi.Plugins.AzureFile
{
    sealed class AzureFileCsiRpcServiceFactory : ICsiRpcServiceFactory
    {
        private readonly IServiceProvider serviceProvider;
        public AzureFileCsiRpcServiceFactory()
        {
            serviceProvider = new ServiceCollection()
               .AddLogging(lb => lb.AddSerillogConsole())
               .AddSingleton<IAzureFileCsiService, AzureFileCsiService>()
               .AddSingleton<IAzureFileServiceFactory, AzureFileServiceFactory>()
               .AddSingleton<IVolumeIdProvider, VolumeIdProvider>()
               .AddSingleton<IAzureFileAccountProvider>(sp =>
               {
                   var defaultProvider = ActivatorUtilities.CreateInstance<AzureFileAccountProviderEnvironment>(sp);
                   var dynamicProvider = ActivatorUtilities.CreateInstance<AzureFileAccountProviderDynamicSecret>(sp);
                   dynamicProvider.Next = defaultProvider;
                   var validator = ActivatorUtilities.CreateInstance<AzureFileAccountProviderValidator>(sp);
                   validator.Next = dynamicProvider;

                   return validator;
               })
               .AddExternalRunner()
               .AddSmbShareAttacher()
               .BuildServiceProvider();
        }

        public Identity.IdentityBase CreateIdentityRpcService() =>
            new IdentityRpcService(Constants.Name, Constants.Version);

        public Controller.ControllerBase CreateControllerRpcService() =>
            ActivatorUtilities.CreateInstance<AzureFileControllerRpcService>(serviceProvider);

        public Node.NodeBase CreateNodeRpcService()
            => ActivatorUtilities.CreateInstance<AzureFileNodeRpcService>(serviceProvider, getNodeIdFromEnv());

        private static string getNodeIdFromEnv() => Environment.GetEnvironmentVariable("NODE_ID")
            ?? Environment.MachineName;
    }
}
