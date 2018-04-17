using System;
using Csi.Helpers.Azure;
using Csi.V0;
using Csi.V0.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Csi.Plugins.AzureFile
{
    sealed class AzureFileCsiRpcServer : CsiRpcServer
    {
        private readonly IServiceProvider serviceProvider;
        public AzureFileCsiRpcServer()
        {
            serviceProvider = new ServiceCollection()
               .AddLogging(lb => lb.AddSerillogConsole())
               .AddSingleton<IAzureFileCsiService, AzureFileCsiService>()
               .AddSingleton<IAzureFileServiceFactory, AzureFileServiceFactory>()
               .AddSingleton<ICsiRpcServer, AzureFileCsiRpcServer>()
               .AddSingleton<IVolumeIdProvider, VolumeIdProvider>()
               .AddSingleton<IAzureFileAccountProvider>(sp => {
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

        public override Identity.IdentityBase CreateIdentityRpcService() =>
            new IdentityRpcService(Constants.Name, Constants.Version);

        public override Controller.ControllerBase CreateControllerRpcService() =>
            ActivatorUtilities.CreateInstance<AzureFileControllerRpcService>(serviceProvider);

        public override Node.NodeBase CreateNodeRpcService()
            => ActivatorUtilities.CreateInstance<AzureFileNodeRpcService>(serviceProvider, getNodeIdFromEnv());

        private static string getNodeIdFromEnv() => Environment.GetEnvironmentVariable("NODE_ID") 
            ?? Environment.MachineName;
    }
}
