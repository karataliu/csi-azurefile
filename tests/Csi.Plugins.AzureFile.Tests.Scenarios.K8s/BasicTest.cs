using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Csi.Plugins.AzureFile.Tests.Scenarios.K8s
{
    public class BasicTest
    {
        private static readonly ILoggerFactory loggerFactory = TestHelper.CreateLoggerFactory();

        [Fact]
        public async Task PodWithPvc()
        {
            var testNamespace = Guid.NewGuid().ToString(); ;
            var tkc = new TestKubernetesClient(testNamespace, loggerFactory.CreateLogger<TestKubernetesClient>());

            var pvcName = "pvc1";
            var pvc = new V1PersistentVolumeClaim
            {
                Metadata = new V1ObjectMeta
                {
                    Name = pvcName,
                },
                Spec = new V1PersistentVolumeClaimSpec
                {
                    AccessModes = new List<string> { "ReadWriteOnce" },
                    StorageClassName = "azurefile-csi",
                    Resources = new V1ResourceRequirements
                    {
                        Requests = new Dictionary<string, ResourceQuantity>
                        {
                            ["storage"] = new ResourceQuantity("2Gi"),
                        },
                    },

                },
            };
            var pod = new V1Pod
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "claim",
                },
                Spec = new V1PodSpec
                {
                    RestartPolicy = "Never",
                    Containers = new List<V1Container>
                    {
                        new V1Container
                        {
                            Image = "busybox",
                            Args = new List<string>{"ls", "/" },
                            Name ="test",
                            VolumeMounts = new List<V1VolumeMount>
                            {
                                new V1VolumeMount
                                {
                                    MountPath ="/af-vol",
                                    Name="af-pvc",
                                }
                            }
                        }
                    },
                    Volumes = new List<V1Volume>
                    {
                        new V1Volume
                        {
                            Name = "af-pvc",
                            PersistentVolumeClaim = new V1PersistentVolumeClaimVolumeSource
                            {
                                ClaimName = pvcName
                            },
                        }
                    }
                }
            };

            await tkc.EnsureNamespace();
            await tkc.CreatePvc(pvc);
            await tkc.CreatePod(pod);

            // TODO verify pod status, share content, and do clean up
        }
    }
}
