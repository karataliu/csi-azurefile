using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;

namespace Csi.AzureFile
{
    static class SmbShareAttacherExtensions
    {
        public static IServiceCollection AddSmbShareAttacher(this IServiceCollection sc)
            => sc.AddSingleton(typeof(ISmbShareAttacher), getType());

        public static Type getType()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return typeof(SmbShareAttacherLinux);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return typeof(SmbShareAttacherWindows);
            }
            throw new NotImplementedException();
        }
    }
}
