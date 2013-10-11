using Cache;
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
//using WebAPI.Models;
using Operations;

namespace WebAPI.Controllers
{
    public class FileApiController : ApiController
    {
        //private IBlobRepository _blobRepository;
        //private IAzureCache _azureCache;

        private IOperations _operations;

        public FileApiController(IOperations Operations)
        {
            _operations = Operations;
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

                using (Stream fileChunkStream = await provider.Contents[0].ReadAsStreamAsync())
                {

                    //Check for not null or empty
                    if (fileChunkStream == null)
                        throw new HttpResponseException(HttpStatusCode.NotFound);

                    // Read file chunk detail
                    FileChunk chunk = provider.Contents[0].Headers.GetMetaData();
                    chunk.ChunkData = fileChunkStream.ReadFully();

                    // Get saved file bytes using LocalFileName. Put it in the putblock.
                        // Update Dictionary with FileId - PutblockId
                    _operations.UploadChunk(chunk);

                    // check for last chunk, if so, then do a PubBlockList
                    // Remove all keys of that FileID from Dictionary
                    if (chunk.IsCompleted)
                        _operations.CommitChunks(chunk);


                    fileChunkStream.Dispose();
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