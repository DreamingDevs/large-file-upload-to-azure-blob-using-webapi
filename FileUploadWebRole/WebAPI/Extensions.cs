using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using WebAPI.Models;

namespace WebAPI
{
    public static class Extensions
    {
        public static FileChunk GetMetaData(this HttpContentHeaders headers)
        {
            return new FileChunk()
            {
                ChunkId = Convert.ToBase64String(Encoding.UTF8.GetBytes(headers.Where(p => p.Key == "ChunkId").First().Value.First())),
                FileId = headers.Where(p => p.Key == "FileId").First().Value.First(),
                IsCompleted = Boolean.Parse(headers.Where(p => p.Key == "IsCompleted").First().Value.First())
            };
        }
    }
}