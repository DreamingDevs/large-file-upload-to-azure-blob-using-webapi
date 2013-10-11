using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BlobRepository : IBlobRepository
    {
        public bool UploadBlock(string fileId, string blockId, Stream data)
        {
            CloudBlobContainer container = BlobUtilities.GetBlobClient.GetContainerReference(BlobUtilities.Container);
            container.CreateIfNotExists();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileId);

            blockBlob.PutBlock(blockId, data, null);
            return true;
        }

        public void CommintBlocks(string fileId, List<string> blockIds)
        {
            CloudBlobContainer container = BlobUtilities.GetBlobClient.GetContainerReference(BlobUtilities.Container);
            container.CreateIfNotExists();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileId);

            blockBlob.PutBlockListAsync(blockIds);
        }
    }
    internal class BlobUtilities
    {
        public static CloudBlobClient GetBlobClient
        {
            get
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                return blobClient;
            }
        }

        public static string Container
        {
            get
            {
                return CloudConfigurationManager.GetSetting("StorageContainer");
            }
        }
    }
}
