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
                Console.WriteLine(String.Format("File ID -> {0}", fileId));



                //***************************** Split a big file in to List of Byte arrays *****************************//

                // Reall File data to byte chunks of 100KB.
                Dictionary<int, byte[]> chunks = new Dictionary<int, byte[]>();
                int bufferSize = 1000 * 1024;
                byte[] buffer;

                using (FileStream fileData = File.OpenRead(@"c:\Users\aisadmin\Desktop\Me\2.flv"))
                {
                    int index = 0;
                    int i = 1;
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

                        chunks.Add(i, buffer);
                        index = (int)fileData.Position;
                        i++;
                    }
                }


                //***************************** Send individual Byte arrays to FileApi Service *****************************//

                // Send chunks to WebApi endpoint for commits
                Console.WriteLine("Uploading Chunks - Started");
                int count = 1;
                while (chunks.Count() != 0)
                {
                    var chunk = chunks.First();
                    var fileContent = new ByteArrayContent(chunk.Value);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Sample" };
                    fileContent.Headers.Add("FileId", fileId);
                    fileContent.Headers.Add("ChunkId", chunk.Key.ToString("d8"));

                    if (chunks.Count() == 1)
                        fileContent.Headers.Add("IsCompleted", "true");
                    else
                        fileContent.Headers.Add("IsCompleted", "false");

                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(fileContent);

                        // Make a call to Web API 
                        var result = client.PostAsync("/api/fileapi", content).Result;

                        if (result.StatusCode == System.Net.HttpStatusCode.OK)
                            chunks.Remove(chunk.Key);

                        Console.WriteLine(String.Format("Chunk{0} with {1} bytes size. Status:{2}", count.ToString(), chunk.Value.Length, msg.StatusCode));
                    }

                    count++;
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
