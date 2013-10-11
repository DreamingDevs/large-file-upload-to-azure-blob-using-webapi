using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.IO;

namespace Operations
{
    public static class Extensions
    {
        public static FileChunk GetMetaData(this HttpContentHeaders headers)
        {
            return new FileChunk()
            {
                ChunkId = Convert.ToBase64String(Encoding.UTF8.GetBytes(headers.Where(p => p.Key == "ChunkId").First().Value.First())),
                FileId = headers.Where(p => p.Key == "FileId").First().Value.First(),
                IsCompleted = Boolean.Parse(headers.Where(p => p.Key == "IsCompleted").First().Value.First()),
                OriginalChunkId = headers.Where(p => p.Key == "ChunkId").First().Value.First()
            };
        }

        public static byte[] ReadFully(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
