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
        public bool UploadBlock(string FileId, string BlockId, Stream Data)
        {
            CloudBlobContainer container = BlobUtilities.GetBlobClient.GetContainerReference(BlobUtilities.Container);
            container.CreateIfNotExists();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileId);

            string blockID = Convert.ToBase64String(Encoding.UTF8.GetBytes(BlockId.ToString()));
            blockBlob.PutBlock(blockID, Data, null);
            return true;
        }

        public bool CommintBlocks(string FileId, List<string> BlockIds)
        {
            CloudBlobContainer container = BlobUtilities.GetBlobClient.GetContainerReference(BlobUtilities.Container);
            container.CreateIfNotExists();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileId);

            blockBlob.PutBlockList(BlockIds);
            return true;
        }
    }
    internal class BlobUtilities
    {
        public static CloudBlobClient GetBlobClient
        {
            get
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("Connection Stirng goes here");
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                return blobClient;
            }
        }

        public static string Container
        {
            get
            {
                return "Files";
            }
        }
    }
}
