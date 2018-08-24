using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Threading;


namespace Csi.Plugins.AzureFile.Tests.Scenarios.K8s
{
    public class BasicTest: IDisposable
    {
        private static readonly ILoggerFactory loggerFactory = TestHelper.CreateLoggerFactory();
        private readonly ILogger logger;
        private TestKubernetesClient tkc;
        private TestAzureFile azureFile;

        public BasicTest(){
            this.logger = loggerFactory.CreateLogger<BasicTest>();
            var testNamespace = "e2e-tests-csi-file-" + Guid.NewGuid().ToString();
            this.tkc = new TestKubernetesClient(testNamespace, loggerFactory.CreateLogger<TestKubernetesClient>());
            this.azureFile = new TestAzureFile();
        }

        [Fact]
        public async Task CsiAzureFileTest()
        {
            STEP("Init the environment and attach azure file to pod");
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
                            Args = new List<string>{"touch", "/af-vol/temp"},
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

            var originalSharesList = await azureFile.GetSharesList();
            await tkc.EnsureNamespace();
            await tkc.CreatePvc(pvc);
            await tkc.CreatePod(pod);
            Assert.True(await tkc.WaitPodCompleted(pod.Metadata.Name), "Failed to construct the pod");

            STEP("Get the name of the new share");
            var tempSharesList = await azureFile.GetSharesList();
            Assert.Equal(tempSharesList.Count, originalSharesList.Count + 1);
            string newShareName = "";
            foreach(var shareResult in tempSharesList){
                bool isfind = false;
                foreach(var oldShareResult in originalSharesList){
                    if(oldShareResult.Name == shareResult.Name){
                        isfind = true;
                        break;
                    }
                }
                if(!isfind){
                    newShareName = shareResult.Name;
                    break;
                }
            }
            Assert.NotEqual(newShareName, "");

            STEP("Validate if the created file in the share");
            string targetFileName = "temp";
            bool isFindFile = false;
            var filesList = await azureFile.GetRootDirectoryFilesList(newShareName);
            foreach(var file in filesList){
                var absPath = file.Uri.AbsolutePath;
                var fileName = absPath.Substring(absPath.LastIndexOf('/') + 1);
                if(fileName == targetFileName){
                    isFindFile = true;
                }
            }
            Assert.True(isFindFile, "Fail to find the file in the share");
            
            STEP("Validate the dynamic deletion of share");
            await tkc.DeleteNamespaceIfExists();
            bool isDeleted = false;
            for (int i =0; i<= 10 *60; i+=5){
                Thread.Sleep(5*1000);
                tempSharesList = await azureFile.GetSharesList();
                if (tempSharesList.Count == originalSharesList.Count){
                    isDeleted = true;
                    break;
                }
            }
            Assert.True(isDeleted, "Fail to dynamically delete the share");
        }

        private void STEP(string inf){
            logger.LogInformation("E2E TEST STEP: {0}", inf);
        }

        public async void Dispose()
        {
            STEP("Cleaning up");
            await tkc.DeleteNamespaceIfExists();
            this.tkc = null;
            this.azureFile = null;
        }
    }
}
