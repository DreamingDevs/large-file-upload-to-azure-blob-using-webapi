using Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class FileApiController : ApiController
    {
        static List<FileChunk> fileChunkTracker = new List<FileChunk>();
        private IBlobRepository _blobRepository;
        public FileApiController(IBlobRepository BlobRepository)
        {
            _blobRepository = BlobRepository;
        }

        public string Get()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<HttpResponseMessage> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Temp storage location for File Chunks
            MultipartMemoryStreamProvider provider = new MultipartMemoryStreamProvider();

            try
            {
                // Read all contents of multipart message into MultipartMemoryStreamProvider.                 
                await Request.Content.ReadAsMultipartAsync(provider);
                Stream fileChunk = await provider.Contents[0].ReadAsStreamAsync();                

                //Check for not null or empty
                if (fileChunk == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                // Read file chunk detail
                FileChunk chunk = provider.Contents[0].Headers.GetMetaData();

                // Get saved file bytes using LocalFileName. Put it in the putblock.
                // Update Dictionary with FileId - PutblockId
                _blobRepository.UploadBlock(chunk.FileId, chunk.ChunkId, fileChunk);
                fileChunkTracker.Add(chunk);

                // check for last chunk, if so, then do a PubBlockList
                // Remove all keys of that FileID from Dictionary
                if (chunk.IsCompleted)
                {
                    Dictionary<string, string> blockIds = fileChunkTracker.Where(p => p.FileId == chunk.FileId)
                                                                          .Select(p => new { p.OriginalChunkId, p.ChunkId })
                                                                          .ToDictionary( d => d.OriginalChunkId, d => d.ChunkId);

                    var comparer = new BlockIdComparer();
                    blockIds.OrderBy(p => p.Key,comparer);
                    _blobRepository.CommintBlocks(chunk.FileId, blockIds.Select(p => p.Value).ToList());
                    fileChunkTracker.RemoveAll(p => p.FileId == chunk.FileId);
                }

                // Send OK Response along with saved file names to the client.                 
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}