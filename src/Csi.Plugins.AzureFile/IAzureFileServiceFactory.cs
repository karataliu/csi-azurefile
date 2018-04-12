namespace Csi.Plugins.AzureFile
{
    interface IAzureFileServiceFactory
    {
        IAzureFileService Create(AzureFileAccount azureFileAccount);
    }
}
