using System;
using System.Collections.Generic;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace Csi.Plugins.AzureFile
{
    static class AzureEnvironmentHelper
    {
        private static Dictionary<string, AzureEnvironment> envDic =
            new Dictionary<string, AzureEnvironment>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(AzureEnvironment.AzureGlobalCloud)] = AzureEnvironment.AzureGlobalCloud,
                [nameof(AzureEnvironment.AzureChinaCloud)] = AzureEnvironment.AzureChinaCloud,
                [nameof(AzureEnvironment.AzureUSGovernment)] = AzureEnvironment.AzureUSGovernment,
                [nameof(AzureEnvironment.AzureGermanCloud)] = AzureEnvironment.AzureGermanCloud,
            };

        public static string GetStorageEndpointSuffix(string environmentName)
            => parseAzureEnvironment(environmentName).StorageEndpointSuffix.TrimStart('.');

        private static AzureEnvironment parseAzureEnvironment(string environmentName)
        {
            if (string.IsNullOrEmpty(environmentName)) return AzureEnvironment.AzureGlobalCloud;
            if (envDic.TryGetValue(environmentName, out var env)) return env;
            throw new Exception("Unknown environment: " + environmentName);
        }
    }
}
