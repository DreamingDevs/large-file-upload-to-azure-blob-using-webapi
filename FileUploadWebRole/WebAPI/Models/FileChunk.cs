using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{

    public class FileChunk
    {
        public string FileId { get; set; }
        public string ChunkId { get; set; }
        public double Size { get; set; }
        public bool IsCompleted { get; set; }
        public string OriginalChunkId { get; set; }
    }
}