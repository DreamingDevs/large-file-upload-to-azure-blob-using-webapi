using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operations
{
    public interface IOperations
    {
        void UploadChunk(FileChunk chunk, Stream data);

        void CommitChunks(FileChunk chunk);
    }
}
