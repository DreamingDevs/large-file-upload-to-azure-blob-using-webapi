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
            FileChunk chunk = null;

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
                    chunk = provider.Contents[0].Headers.GetMetaData();
                    chunk.ChunkData = fileChunkStream.ReadFully();

                    // Upload Chunk to blob storage and store the reference in Azure Cache
                    _operations.UploadChunk(chunk);

                    // check for last chunk, if so, then do a PubBlockList
                    // Remove all keys of that FileID from Dictionary
                    if (chunk.IsCompleted)
                        _operations.CommitChunks(chunk);
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