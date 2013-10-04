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

namespace WebAPI.Controllers
{
    public class FileApiController : ApiController
    {
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
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/App_Data");
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation); 

            try 
            { 
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider. 
                await Request.Content.ReadAsMultipartAsync(provider); 
                
                foreach (MultipartFileData file in provider.FileData) 
                {
                    Stream s = provider.GetStream(provider.Contents[0], provider.Contents[0].Headers);
                    // Get Bytes here....and push it to blob
                } 
                
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

            public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
            {
                return base.GetStream(parent, headers);
            }
        }

          
    }
}