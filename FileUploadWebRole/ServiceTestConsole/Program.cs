using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                // Make sure to change API address 
                client.BaseAddress = new Uri("http://127.0.0.1:81/");

                // Get the File Id
                var init = client.GetStringAsync("/api/fileapi").Result;
                string fileId = init.Trim(new char[] { '"' });
                Console.WriteLine(String.Format("File ID -> {0}",fileId));



                //************************Test To simulate multiple random file chunks commit ************************//

                // Reall File data to byte chunks of 100KB.
                List<byte[]> chunks = new List<byte[]>();
                int bufferSize = 100 * 1024;
                byte[] buffer;

                using (FileStream fileData = File.OpenRead(@"c:\Users\aisadmin\Desktop\Me\NF2202533167366.pdf"))
                {
                    int index = 0;
                    while (fileData.Position < fileData.Length)
                    {

                        if (index + bufferSize > fileData.Length)
                        {
                            buffer = new byte[(int)fileData.Length - index];
                            fileData.Read(buffer, 0, ((int)fileData.Length - index));
                        }
                        else
                        {
                            buffer = new byte[bufferSize];
                            fileData.Read(buffer, 0, bufferSize);
                        }

                        chunks.Add(buffer);
                        index = (int)fileData.Position;
                    }
                }

                // Send chunks to WebApi endpoint for commits
                Console.WriteLine("Uploading Chunks - Started");
                int chunkId = 1;
                foreach (var chunk in chunks)
                {
                    var fileContent = new ByteArrayContent(chunk);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Sample.pdf" };
                    fileContent.Headers.Add("FileId", fileId);
                    fileContent.Headers.Add("ChunkId", chunkId.ToString("d8"));

                    if (chunkId == chunks.Count())
                        fileContent.Headers.Add("IsCompleted", "true");
                    else
                        fileContent.Headers.Add("IsCompleted", "false");

                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(fileContent);

                        // Make a call to Web API 
                        var result = client.PostAsync("/api/fileapi", content).Result;
                        Console.WriteLine(String.Format("Chunk{0} with {1} bytes size. Status:{2}", chunkId.ToString(), chunk.Length, result.StatusCode));
                    }

                    chunkId++;
                }

                // For testing MultiPartFormData
                //var fieldContent = new FormUrlEncodedContent(new[] 
                //{ 
                // new KeyValuePair<string, string>("fileId", "123"),
                // new KeyValuePair<string, string>("ChunkId", "456")
                //});
                //content.Add(fieldContent);
                Console.WriteLine("Uploading Chunks - Completed");
                Console.ReadLine();
            }
        }
    }
}
