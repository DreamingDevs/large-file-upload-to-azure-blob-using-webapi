using Repository;
using System;
using System.Collections.Generic;
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
        static Dictionary<string, string> fileChunkTracker = new Dictionary<string, string>();
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
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/App_Data");
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);

            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider. 
                await Request.Content.ReadAsMultipartAsync(provider);
                MultipartFileData file = provider.FileData[0];

                //Check for not null or empty
                if (file == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);


                // Read file chunk detail
                FileChunk chunk = file.Headers.GetMetaData();
                chunk.FileId = file.Headers.Where(p => p.Key == "FileId").First().Value.First();
                chunk.ChunkId = file.Headers.Where(p => p.Key == "ChunkId").First().Value.First();
                chunk.IsCompleted = Boolean.Parse(file.Headers.Where(p => p.Key == "IsCompleted").First().Value.First());


                // Read File Chunk Bytes and Delete the temp copy
                byte[] fileChunkBytes;
                using (FileStream fs = new FileStream(file.LocalFileName, FileMode.Open, FileAccess.Read))
                {
                    fileChunkBytes = new byte[fs.Length];
                    fs.Read(fileChunkBytes, 0, (int)fs.Length);
                    fs.Close();
                    fs.Dispose();
                }
                System.IO.File.Delete(file.LocalFileName);

                // TODO
                // Get saved file bytes using LocalFileName
                // put it in the putblock
                // Update Dictionary with FileId - PutblockId

                // TODO
                // check for last chunk, if so, then do a PubBlockList
                // Remove all keys of that FileID from Dictionary

                // Send OK Response along with saved file names to the client.                 
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }

    }
}